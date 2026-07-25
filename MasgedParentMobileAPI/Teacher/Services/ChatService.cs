using System.Security.Claims;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Services;

public sealed class ChatService(AppDbContext db, MasgedParentMobileAPI.Services.PushNotificationService pushNotifications)
{
    private const int MaxMessageLength = 2000;

    public async Task<List<ChatConversationDto>> GetTeacherConversationsAsync(int teacherId, int circleId)
    {
        if (!await QuranCircleOwnedByTeacherAsync(circleId, teacherId))
            return new List<ChatConversationDto>();

        var students = await db.RegisterForms.AsNoTracking()
            .Where(s => s.QuranCircleId == circleId)
            .Select(s => new { s.Id, s.StudentName, s.FatherName, s.FatherPhone })
            .ToListAsync();

        if (students.Count == 0)
            return [];

        var studentIds = students.Select(s => s.Id).ToList();

        var msgs = await db.ParentTeacherChatMessages.AsNoTracking()
            .Where(m =>
                m.TeacherId == teacherId &&
                m.StudentId != null &&
                studentIds.Contains(m.StudentId!.Value))
            .ToListAsync();

        var list = students.Select(s =>
        {
            var threadMsgs = msgs.Where(m => m.StudentId == s.Id).ToList();
            var last = threadMsgs.Count == 0
                ? null
                : threadMsgs.OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id).First();
            var unread = threadMsgs.Count(m =>
                !m.IsReadByTeacher &&
                m.SenderType == ChatSenderTypes.Parent);

            var preview = last?.MessageText;
            if (!string.IsNullOrEmpty(preview) && preview.Length > 140)
                preview = preview[..140] + "…";

            var canonPhone = PhoneNormalizer.ToCanonical(string.IsNullOrWhiteSpace(s.FatherPhone)
                ? "00000000"
                : s.FatherPhone);

            return new ChatConversationDto
            {
                ParentPhone = canonPhone,
                StudentId = s.Id,
                StudentName = s.StudentName,
                TeacherId = teacherId,
                ParentDisplayName = s.FatherName,
                LastMessagePreview = preview,
                LastMessageAt = last is null ? null : KuwaitTime.ToOffset(last.SentAt),
                UnreadCount = unread,
            };
        }).ToList();

        return list.OrderByDescending(c => c.LastMessageAt).ToList();
    }

    public async Task<IReadOnlyList<ChatMessageDto>> GetMessagesForTeacherAsync(
        ClaimsPrincipal user,
        int studentId,
        int teacherIdRoute,
        int? beforeMessageId,
        int take)
    {
        if (!TryParseTeacherContext(user, out var teacherId, out var circleId) ||
            teacherId != teacherIdRoute)
            throw new UnauthorizedAccessException();

        if (await ResolveCanonicalParentPhoneForStudentInCircleAsync(circleId, studentId) is null)
            throw new UnauthorizedAccessException();

        var clampedTake = Math.Clamp(take, 1, 100);

        var q = db.ParentTeacherChatMessages.AsNoTracking()
            .Where(m => m.StudentId == studentId && m.TeacherId == teacherId);

        if (beforeMessageId is int bm)
            q = q.Where(m => m.Id < bm);

        var list = await q.OrderByDescending(m => m.Id).Take(clampedTake).ToListAsync();
        list.Reverse();
        return list.Select(MapDto).ToList();
    }

    public async Task<ChatMessageDto> SendTeacherMessageAsync(
        ClaimsPrincipal user,
        int studentId,
        int teacherIdRoute,
        SendChatMessageRequestDto body,
        ChatBroadcastClient broadcast,
        CancellationToken cancellationToken)
    {
        ValidateMessageText(body.MessageText);
        if (!TryParseTeacherContext(user, out var teacherId, out var circleId) ||
            teacherId != teacherIdRoute)
            throw new UnauthorizedAccessException();

        if (body.StudentId is int bsid && bsid != studentId)
            throw new ArgumentException("student mismatch");

        var canon = await ResolveCanonicalParentPhoneForStudentInCircleAsync(circleId, studentId);
        if (canon is null)
            throw new UnauthorizedAccessException();

        var row = new ParentTeacherChatMessage
        {
            ParentPhone = canon,
            TeacherId = teacherId,
            SenderType = ChatSenderTypes.Teacher,
            MessageText = body.MessageText.Trim(),
            StudentId = studentId,
            SentAt = KuwaitTime.Now,
            IsReadByTeacher = true,
            IsReadByParent = false,
        };

        db.ParentTeacherChatMessages.Add(row);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await pushNotifications.SendChatMessageToParentAsync(
            canon,
            teacherId,
            studentId,
            row.MessageText,
            cancellationToken).ConfigureAwait(false);

        var dto = MapDto(row);
        await broadcast.TryBroadcastReceivedMessage(dto, cancellationToken).ConfigureAwait(false);
        return dto;
    }

    public async Task MarkReadAsTeacherAsync(
        ClaimsPrincipal user,
        int studentId,
        int teacherIdRoute,
        MarkChatReadRequestDto? body)
    {
        if (!TryParseTeacherContext(user, out var teacherId, out var circleId) ||
            teacherId != teacherIdRoute)
            throw new UnauthorizedAccessException();

        if (await ResolveCanonicalParentPhoneForStudentInCircleAsync(circleId, studentId) is null)
            throw new UnauthorizedAccessException();

        var q = db.ParentTeacherChatMessages.Where(m =>
            m.StudentId == studentId &&
            m.TeacherId == teacherId &&
            !m.IsReadByTeacher &&
            m.SenderType == ChatSenderTypes.Parent);

        if (body?.UpToMessageId is int up)
            q = q.Where(m => m.Id <= up);

        await q.ExecuteUpdateAsync(s => s.SetProperty(m => m.IsReadByTeacher, true))
            .ConfigureAwait(false);
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

    private Task<bool> QuranCircleOwnedByTeacherAsync(int circleId, int teacherId) =>
        db.QuranCircles.AsNoTracking().AnyAsync(c => c.Id == circleId && c.TeacherId == teacherId);

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
