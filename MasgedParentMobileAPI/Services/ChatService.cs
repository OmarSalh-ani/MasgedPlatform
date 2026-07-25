using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Services;

public sealed class ChatService(NewMasgedTeacherAPIDBContext db, PushNotificationService pushNotifications)
{
    private const int MaxMessageLength = 2000;

    /// <summary>One row per enrolled student ↔ teacher pair for this parent JWT.</summary>
    public async Task<List<ChatConversationDto>> GetConversationsForParentAsync(string jwtFatherCanonical)
    {
        var canonical = PhoneNormalizer.ToCanonical(jwtFatherCanonical);
        var variants = PhoneNormalizer.GetVariants(canonical).ToList();

        var enrolmentsRaw = await db.RegisterForms
            .AsNoTracking()
            .Include(r => r.QuranCircle)
                .ThenInclude(c => c!.Teacher)
            .Where(r =>
                variants.Contains(r.FatherPhone!) ||
                (r.FatherPhone2 != null && variants.Contains(r.FatherPhone2)))
            .Where(r => r.QuranCircle != null && r.QuranCircle.TeacherId != null)
            .Select(r => new
            {
                r.Id,
                r.StudentName,
                FatherPhone = r.FatherPhone ?? string.Empty,
                TeacherId = r.QuranCircle!.TeacherId!.Value,
                TeacherName = r.QuranCircle.Teacher!.Name,
            })
            .ToListAsync();

        var enrolments = enrolmentsRaw
            .Select(e => (
                e.Id,
                e.StudentName,
                e.TeacherId,
                e.TeacherName,
                ParentCanon: PhoneNormalizer.ToCanonical(e.FatherPhone)))
            .ToList();

        var studentIds = enrolments.Select(e => e.Id).Distinct().ToList();
        if (studentIds.Count == 0)
            return [];

        var messages = await db.ParentTeacherChatMessages
            .AsNoTracking()
            .Where(m =>
                m.StudentId != null &&
                studentIds.Contains(m.StudentId.Value))
            .ToListAsync();

        var list = enrolments.Select(e =>
        {
            var threadMsgs = messages.Where(m =>
                m.StudentId == e.Id &&
                m.TeacherId == e.TeacherId).ToList();

            var last = threadMsgs.Count == 0
                ? null
                : threadMsgs.OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id).First();
            var unread = threadMsgs.Count(m =>
                !m.IsReadByParent &&
                m.SenderType == ChatSenderType.Teacher);

            var preview = last?.MessageText;
            if (!string.IsNullOrEmpty(preview) && preview.Length > 140)
                preview = preview[..140] + "…";

            return new ChatConversationDto
            {
                ParentPhone = string.IsNullOrWhiteSpace(e.ParentCanon) ? canonical : e.ParentCanon,
                StudentId = e.Id,
                StudentName = e.StudentName,
                TeacherId = e.TeacherId,
                TeacherName = e.TeacherName,
                LastMessagePreview = preview,
                LastMessageAt = last is null ? null : KuwaitTime.ToOffset(last.SentAt),
                UnreadCount = unread,
            };
        }).ToList();

        return list.OrderByDescending(c => c.LastMessageAt).ToList();
    }

    public async Task<IReadOnlyList<ChatMessageDto>> GetMessagesForParentAsync(
        string jwtFatherCanonical,
        int teacherId,
        int studentId,
        int? beforeMessageId,
        int take = 50)
    {
        var canonical = PhoneNormalizer.ToCanonical(jwtFatherCanonical);
        await EnsureParentOwnsStudentAsync(canonical, studentId);

        var okTeacher = await db.RegisterForms.AsNoTracking()
            .Include(r => r.QuranCircle)
            .Where(r => r.Id == studentId)
            .AnyAsync(r => r.QuranCircle != null && r.QuranCircle.TeacherId == teacherId);

        if (!okTeacher)
            throw new UnauthorizedAccessException();

        return await GetMessagesForStudentTeacherCoreAsync(studentId, teacherId, beforeMessageId, take);
    }

    private async Task<IReadOnlyList<ChatMessageDto>> GetMessagesForStudentTeacherCoreAsync(
        int studentId,
        int teacherId,
        int? beforeMessageId,
        int take)
    {
        var clampedTake = Math.Clamp(take, 1, 100);

        var q = db.ParentTeacherChatMessages
            .AsNoTracking()
            .Where(m => m.StudentId == studentId && m.TeacherId == teacherId);

        if (beforeMessageId is int bm)
            q = q.Where(m => m.Id < bm);

        var list = await q
            .OrderByDescending(m => m.Id)
            .Take(clampedTake)
            .ToListAsync();

        list.Reverse();
        return list.Select(MapDto).ToList();
    }

    public async Task<ChatMessageDto?> SendParentMessageAsync(
        string jwtFatherCanonical,
        int teacherId,
        int studentId,
        SendChatMessageRequest request,
        IChatRealtimePublisher publisher,
        CancellationToken ct = default)
    {
        if (request.StudentId is int bodySid && bodySid != studentId)
            throw new ArgumentException("student mismatch");

        var dto = await SendParentMessagePersistOnlyAsync(
            jwtFatherCanonical, teacherId, studentId, request.MessageText);
        await publisher.PublishReceiveMessage(dto, ct);
        return dto;
    }

    public async Task<ChatMessageDto> SendParentMessagePersistOnlyAsync(
        string jwtFatherPhoneCanonical,
        int teacherId,
        int studentId,
        string messageText)
    {
        ValidateMessageText(messageText);
        var canon = PhoneNormalizer.ToCanonical(jwtFatherPhoneCanonical);
        await EnsureParentCanSendForStudentAsync(canon, teacherId, studentId);

        var row = new ParentTeacherChatMessage
        {
            ParentPhone = canon,
            TeacherId = teacherId,
            SenderType = ChatSenderType.Parent,
            MessageText = messageText.Trim(),
            StudentId = studentId,
            SentAt = KuwaitTime.Now,
            IsReadByParent = true,
            IsReadByTeacher = false,
        };

        db.ParentTeacherChatMessages.Add(row);
        await db.SaveChangesAsync();
        await pushNotifications.SendChatMessageToTeacherAsync(
            teacherId,
            studentId,
            row.MessageText,
            CancellationToken.None);
        return MapDto(row);
    }

    public async Task<ChatMessageDto?> SendTeacherMessageAsync(
        ClaimsPrincipal teacherUser,
        int teacherIdFromRoute,
        int studentId,
        SendChatMessageRequest request,
        IChatRealtimePublisher publisher,
        CancellationToken ct = default)
    {
        try
        {
            if (request.StudentId is int bodySid && bodySid != studentId)
                throw new ArgumentException("student mismatch");

            var dto = await SendTeacherMessagePersistOnlyAsync(
                teacherUser, teacherIdFromRoute, studentId, request.MessageText);
            await publisher.PublishReceiveMessage(dto, ct);
            return dto;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    public async Task<ChatMessageDto> SendTeacherMessagePersistOnlyAsync(
        ClaimsPrincipal teacherUser,
        int teacherIdFromRoute,
        int studentId,
        string messageText)
    {
        ValidateMessageText(messageText);
        if (!TryParseTeacherContext(teacherUser, out var teacherId, out var circleId))
            throw new UnauthorizedAccessException();

        if (teacherId != teacherIdFromRoute)
            throw new UnauthorizedAccessException();

        var canon = await ResolveCanonicalParentPhoneForStudentInCircleAsync(circleId, studentId);
        if (canon is null)
            throw new UnauthorizedAccessException();

        var row = new ParentTeacherChatMessage
        {
            ParentPhone = canon,
            TeacherId = teacherId,
            SenderType = ChatSenderType.Teacher,
            MessageText = messageText.Trim(),
            StudentId = studentId,
            SentAt = KuwaitTime.Now,
            IsReadByTeacher = true,
            IsReadByParent = false,
        };

        db.ParentTeacherChatMessages.Add(row);
        await db.SaveChangesAsync();
        await pushNotifications.SendChatMessageToParentAsync(
            canon,
            teacherId,
            studentId,
            row.MessageText,
            CancellationToken.None);
        return MapDto(row);
    }

    public async Task MarkReadForParentAsync(
        string jwtFatherCanonical,
        int teacherId,
        int studentId,
        int? upToMessageId = null)
    {
        var canon = PhoneNormalizer.ToCanonical(jwtFatherCanonical);
        await EnsureParentCanSendForStudentAsync(canon, teacherId, studentId);

        var q = db.ParentTeacherChatMessages.Where(m =>
            m.StudentId == studentId &&
            m.TeacherId == teacherId &&
            !m.IsReadByParent &&
            m.SenderType == ChatSenderType.Teacher);

        if (upToMessageId is int up)
            q = q.Where(m => m.Id <= up);

        await q.ExecuteUpdateAsync(s => s.SetProperty(m => m.IsReadByParent, true));
    }

    public async Task MarkReadForTeacherAsync(
        ClaimsPrincipal teacherUser,
        int teacherIdFromRoute,
        int studentId,
        int? upToMessageId = null)
    {
        if (!TryParseTeacherContext(teacherUser, out var teacherId, out var circleId))
            throw new UnauthorizedAccessException();
        if (teacherId != teacherIdFromRoute)
            throw new UnauthorizedAccessException();

        if (await ResolveCanonicalParentPhoneForStudentInCircleAsync(circleId, studentId) is null)
            throw new UnauthorizedAccessException();

        var q = db.ParentTeacherChatMessages.Where(m =>
            m.StudentId == studentId &&
            m.TeacherId == teacherId &&
            !m.IsReadByTeacher &&
            m.SenderType == ChatSenderType.Parent);

        if (upToMessageId is int up)
            q = q.Where(m => m.Id <= up);

        await q.ExecuteUpdateAsync(s => s.SetProperty(m => m.IsReadByTeacher, true));
    }

    /// <summary>SignalR joins; parent JWT has fatherPhone; teacher JWT has circleId.</summary>
    public async Task<bool> CanUserJoinConversationAsync(
        ClaimsPrincipal user,
        int studentId,
        int teacherId)
    {
        var fatherPhoneClaim = user.FindFirstValue("fatherPhone");
        if (!string.IsNullOrEmpty(fatherPhoneClaim))
        {
            var jwtCanon = PhoneNormalizer.ToCanonical(fatherPhoneClaim);
            var phoneVariants = PhoneNormalizer.GetVariants(jwtCanon).ToList();

            return await db.RegisterForms.AsNoTracking()
                .Include(r => r.QuranCircle)
                .Where(r => r.Id == studentId)
                .Where(r =>
                    phoneVariants.Contains(r.FatherPhone!) ||
                    (r.FatherPhone2 != null && phoneVariants.Contains(r.FatherPhone2)))
                .AnyAsync(r => r.QuranCircle != null && r.QuranCircle.TeacherId == teacherId);
        }

        if (!TryParseTeacherContext(user, out var tid, out var circleId))
            return false;

        if (tid != teacherId)
            return false;

        return await ResolveCanonicalParentPhoneForStudentInCircleAsync(circleId, studentId) != null;
    }

    public async Task<bool> ParentCanTalkToTeacherAsync(string canonicalJwtPhone, int teacherId)
    {
        var phoneVariants = PhoneNormalizer.GetVariants(canonicalJwtPhone).ToList();
        return await db.RegisterForms
            .AsNoTracking()
            .Include(r => r.QuranCircle)
            .Where(r =>
                r.QuranCircle != null &&
                r.QuranCircle.TeacherId == teacherId)
            .Where(r =>
                phoneVariants.Contains(r.FatherPhone!) ||
                (r.FatherPhone2 != null && phoneVariants.Contains(r.FatherPhone2)))
            .AnyAsync();
    }

    private async Task EnsureParentCanSendForStudentAsync(
        string parentCanonical,
        int teacherId,
        int studentId)
    {
        await EnsureParentOwnsStudentAsync(parentCanonical, studentId);

        var ok = await db.RegisterForms.AsNoTracking()
            .Include(r => r.QuranCircle)
            .Where(r => r.Id == studentId)
            .AnyAsync(r => r.QuranCircle != null && r.QuranCircle.TeacherId == teacherId);

        if (!ok)
            throw new UnauthorizedAccessException();
    }

    private async Task<string?> ResolveCanonicalParentPhoneForStudentInCircleAsync(
        int circleId,
        int studentId)
    {
        var raw = await db.RegisterForms.AsNoTracking()
            .Where(s => s.Id == studentId && s.QuranCircleId == circleId)
            .Select(s => s.FatherPhone)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        return PhoneNormalizer.ToCanonical(raw);
    }

    private async Task EnsureParentOwnsStudentAsync(string parentCanonical, int studentId)
    {
        var variants = PhoneNormalizer.GetVariants(parentCanonical).ToList();
        var ok = await db.RegisterForms.AsNoTracking()
            .AnyAsync(r =>
                r.Id == studentId &&
                (variants.Contains(r.FatherPhone!) ||
                 (r.FatherPhone2 != null && variants.Contains(r.FatherPhone2))));
        if (!ok)
            throw new UnauthorizedAccessException();
    }

    private static ChatMessageDto MapDto(ParentTeacherChatMessage row) =>
        new()
        {
            Id = row.Id,
            ParentPhone = row.ParentPhone,
            TeacherId = row.TeacherId,
            SenderType = row.SenderType,
            MessageText = row.MessageText,
            StudentId = row.StudentId,
            SentAt = KuwaitTime.ToOffset(row.SentAt),
        };

    private static void ValidateMessageText(string messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText))
            throw new ArgumentException("empty");
        if (messageText.Length > MaxMessageLength)
            throw new ArgumentException("too-long");
    }

    public static bool TryParseTeacherContext(ClaimsPrincipal user, out int teacherId, out int circleId)
    {
        teacherId = 0;
        circleId = 0;
        var idClaim = user.FindFirstValue("id") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        var circleClaim = user.FindFirstValue("circleId");
        return int.TryParse(idClaim, out teacherId) && teacherId > 0 &&
               int.TryParse(circleClaim, out circleId) && circleId > 0;
    }
}
