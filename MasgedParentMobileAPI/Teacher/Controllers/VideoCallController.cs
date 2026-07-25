using System.Security.Claims;
using System.Text.RegularExpressions;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Enums;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VideoCallController(
    AppDbContext db,
    AgoraTokenService agoraTokens,
    PushNotificationService pushNotifications,
    IVideoCallTerminationService videoCallTermination) : ControllerBase
{
    private static string ChannelNameForMeetingId(int id) => $"masged_{id}";

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest(
                "لم يتم العثور على معرف الحلقة. يرجى تسجيل الدخول مرة أخرى."));

        var students = await db.RegisterForms
            .AsNoTracking()
            .Where(x => x.QuranCircleId == circleId)
            .OrderBy(x => x.StudentName)
            .Select(x => new VideoCallStudentDto { Id = x.Id, StudentName = x.StudentName })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new VideoCallStudentsResponseDto
        {
            TeacherName = User.FindFirstValue("name") ?? "المعلم",
            Students = students
        }));
    }

    [HttpGet]
    public async Task<IActionResult> GetMeetings(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var meetings = await db.MeetingsInfo
            .AsNoTracking()
            .Where(m => m.TeacherId == teacherId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var items = meetings.Select(m => new VideoCallListItemDto
        {
            Id = m.Id,
            MeetingName = m.MeetingName ?? string.Empty,
            StartDateTime = m.StartDateTime,
            ChannelName = m.MeetingUrl ?? string.Empty,
            StudentIds = m.StudentIds,
            StudentNames = ResolveStudentNames(m.StudentIds, circleId),
            Status = (byte)m.Status,
            EndedAt = m.EndedAt,
            TeacherNotes = m.TeacherNotes,
        }).ToList();

        return this.ToActionResult(GlobalResponse.Ok(items));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAndStart(
        [FromBody] CreateVideoCallRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var meetingName = string.IsNullOrWhiteSpace(request.MeetingName)
            ? "مكالمة فيديو"
            : request.MeetingName.Trim();

        var startDateTime = request.StartDateTime ?? KuwaitTime.Now;
        var teacherName = request.TeacherName ?? User.FindFirstValue("name") ?? "المعلم";
        var studentIdsStr = request.StudentIds.Count > 0
            ? string.Join(",", request.StudentIds)
            : null;

        var meeting = new MeetingInfo
        {
            TeacherId = teacherId,
            MeetingUrl = "_pending",
            ApiResponse = null,
            MeetingName = meetingName,
            TeacherName = teacherName,
            StartDateTime = startDateTime,
            StudentIds = studentIdsStr,
            CreatedAt = KuwaitTime.Now,
            Status = MeetingStatus.Active,
        };

        db.MeetingsInfo.Add(meeting);
        await db.SaveChangesAsync(cancellationToken);

        var channel = ChannelNameForMeetingId(meeting.Id);
        meeting.MeetingUrl = channel;
        await db.SaveChangesAsync(cancellationToken);

        string joinHint =
            "افتح تطبيق ولي الأمر ← الإشعارات ← اضغط على المكالمة للانضمام.";
        if (request.SendWhatsApp && request.StudentIds.Count > 0)
            await QueueWhatsAppMessagesAsync(
                request.StudentIds,
                circleId,
                teacherName,
                joinHint,
                meetingName,
                startDateTime,
                cancellationToken);

        if (request.StudentIds.Count > 0)
        {
            var fatherPhones = await db.RegisterForms
                .AsNoTracking()
                .Where(s => request.StudentIds.Contains(s.Id) && s.QuranCircleId == circleId)
                .Select(s => s.FatherPhone)
                .Where(p => p != null && p != string.Empty)
                .Distinct()
                .ToListAsync(cancellationToken);

            await pushNotifications.SendVideoCallInviteAsync(
                fatherPhones,
                meeting.Id,
                meetingName,
                teacherName,
                cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);

        var teacherUid = VideoCallUidRules.TeacherUid(teacherId);
        string token;
        try
        {
            token = agoraTokens.BuildRtcPublisherToken(channel, teacherUid);
        }
        catch (InvalidOperationException ex)
        {
            return this.ToActionResult(GlobalResponse.BadRequest(ex.Message));
        }

        return this.ToActionResult(GlobalResponse.Ok(new CreateVideoCallResponseDto
        {
            Id = meeting.Id,
            ChannelName = channel,
            Token = token,
            Uid = teacherUid,
            MeetingName = meetingName,
            Message = "تم إنشاء المكالمة"
        }));
    }

    /// <summary>Refresh RTC token when rejoining a saved meeting.</summary>
    [HttpPost("{id:int}/token")]
    public async Task<IActionResult> RefreshToken(int id, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out _))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var meeting = await db.MeetingsInfo
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.TeacherId == teacherId, cancellationToken);

        if (meeting is null)
            return this.ToActionResult(GlobalResponse.NotFound("المكالمة غير موجودة"));

        if (meeting.Status == MeetingStatus.Ended)
            return this.ToActionResult(GlobalResponse.BadRequest("انتهت هذه المكالمة ولا يمكن الانضمام إليها."));

        var channel = meeting.MeetingUrl?.Trim();
        if (string.IsNullOrEmpty(channel) || channel == "_pending")
            return this.ToActionResult(GlobalResponse.BadRequest("قناة المكالمة غير صالحة"));

        var teacherUid = VideoCallUidRules.TeacherUid(teacherId);
        string token;
        try
        {
            token = agoraTokens.BuildRtcPublisherToken(channel, teacherUid);
        }
        catch (InvalidOperationException ex)
        {
            return this.ToActionResult(GlobalResponse.BadRequest(ex.Message));
        }

        return this.ToActionResult(GlobalResponse.Ok(new VideoCallTokenResponseDto
        {
            ChannelName = channel,
            Token = token,
            Uid = teacherUid
        }));
    }

    /// <summary>Add students to an active meeting and notify their parents.</summary>
    [HttpPost("{id:int}/students")]
    public async Task<IActionResult> AddStudents(
        int id,
        [FromBody] AddStudentsToVideoCallRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (request.StudentIds.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("اختر طالباً واحداً على الأقل."));

        var meeting = await db.MeetingsInfo
            .FirstOrDefaultAsync(m => m.Id == id && m.TeacherId == teacherId, cancellationToken);

        if (meeting is null)
            return this.ToActionResult(GlobalResponse.NotFound("المكالمة غير موجودة"));

        if (meeting.Status == MeetingStatus.Ended)
            return this.ToActionResult(GlobalResponse.BadRequest("انتهت هذه المكالمة."));

        var existingIds = ParseStudentIdList(meeting.StudentIds);
        var validNewIds = await db.RegisterForms
            .AsNoTracking()
            .Where(s => request.StudentIds.Contains(s.Id) && s.QuranCircleId == circleId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var toAdd = validNewIds.Where(sid => !existingIds.Contains(sid)).Distinct().ToList();
        if (toAdd.Count == 0)
            return this.ToActionResult(GlobalResponse.Ok(message: "الطلاب المحددون مدعوون بالفعل."));

        existingIds.AddRange(toAdd);
        meeting.StudentIds = string.Join(",", existingIds);
        await db.SaveChangesAsync(cancellationToken);

        var teacherName = meeting.TeacherName ?? User.FindFirstValue("name") ?? "المعلم";
        var meetingName = meeting.MeetingName ?? "مكالمة فيديو";
        var startDateTime = meeting.StartDateTime;

        string joinHint =
            "افتح تطبيق ولي الأمر ← الإشعارات ← اضغط على المكالمة للانضمام.";

        if (request.SendWhatsApp)
            await QueueWhatsAppMessagesAsync(
                toAdd,
                circleId,
                teacherName,
                joinHint,
                meetingName,
                startDateTime,
                cancellationToken);

        var fatherPhones = await db.RegisterForms
            .AsNoTracking()
            .Where(s => toAdd.Contains(s.Id) && s.QuranCircleId == circleId)
            .Select(s => s.FatherPhone)
            .Where(p => p != null && p != string.Empty)
            .Distinct()
            .ToListAsync(cancellationToken);

        await pushNotifications.SendVideoCallInviteAsync(
            fatherPhones,
            meeting.Id,
            meetingName,
            teacherName,
            cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(
            new { addedCount = toAdd.Count, studentIds = toAdd },
            message: $"تمت دعوة {toAdd.Count} طالب"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMeeting(
        int id,
        [FromBody] UpdateVideoCallRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out _))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var meeting = await db.MeetingsInfo
            .FirstOrDefaultAsync(m => m.Id == id && m.TeacherId == teacherId, cancellationToken);

        if (meeting is null)
            return this.ToActionResult(GlobalResponse.NotFound("المكالمة غير موجودة"));

        meeting.MeetingName = request.MeetingName;
        meeting.StartDateTime = request.StartDateTime ?? KuwaitTime.Now;

        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم التعديل بنجاح"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMeeting(int id, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out _))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var meeting = await db.MeetingsInfo
            .FirstOrDefaultAsync(m => m.Id == id && m.TeacherId == teacherId, cancellationToken);

        if (meeting is null)
            return this.ToActionResult(GlobalResponse.NotFound("المكالمة غير موجودة"));

        if (meeting.Status != MeetingStatus.Ended)
        {
            await videoCallTermination.TerminateMeetingAsync(
                id,
                teacherId,
                meeting.TeacherNotes,
                cancellationToken);
        }

        db.MeetingsInfo.Remove(meeting);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم الحذف بنجاح"));
    }

    private async Task QueueWhatsAppMessagesAsync(
        List<int> selectedStudentIds,
        int circleId,
        string teacherName,
        string joinHint,
        string meetingName,
        DateTime startDateTime,
        CancellationToken cancellationToken)
    {
        var circleName = await db.QuranCircles
            .AsNoTracking()
            .Where(c => c.Id == circleId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var config = await db.WhatsappPreConfiguredMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Event == WhatsappMessageEvent.VideoCallCreated.ToString()
                    || x.Event == "GoogleMeetCreated",
                cancellationToken);

        if (config is null || !config.IsEnabled)
            return;

        var studentsToMsg = await db.RegisterForms
            .Where(s => selectedStudentIds.Contains(s.Id) && s.QuranCircleId == circleId)
            .ToListAsync(cancellationToken);

        foreach (var student in studentsToMsg)
        {
            if (string.IsNullOrEmpty(student.FatherPhone))
                continue;

            var tokens = new Dictionary<string, string>
            {
                { "اسم الطالب", student.StudentName },
                { "رقم الطالب", student.Id.ToString() },
                { "اسم الأب", student.FatherName ?? "" },
                { "اسم الحلقة", circleName },
                { "اسم المعلم", teacherName },
                { "التاريخ", startDateTime.ToString("dd-MM-yyyy") },
                { "الوقت", startDateTime.ToString("hh:mm tt") },
                { "رابط الاجتماع", joinHint },
                { "اسم الاجتماع", meetingName }
            };

            var formattedMessage = WhatsappMessageHelper.FormatMessage(config.WhatsappMessage, tokens);

            if (string.IsNullOrEmpty(formattedMessage))
                continue;

            db.WhatsappTempTables.Add(new WhatsappTempTable
            {
                mobile = student.FatherPhone,
                IsGirl = 0,
                message = formattedMessage
            });
        }
    }

    private static List<int> ParseStudentIdList(string? studentIdsStr)
    {
        if (string.IsNullOrWhiteSpace(studentIdsStr))
            return [];

        return studentIdsStr
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => int.TryParse(s, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
    }

    private string ResolveStudentNames(string? studentIdsStr, int circleId)
    {
        if (string.IsNullOrWhiteSpace(studentIdsStr) || circleId <= 0)
            return string.Empty;

        var ids = ParseStudentIdList(studentIdsStr);

        if (ids.Count == 0)
            return string.Empty;

        var names = db.RegisterForms
            .AsNoTracking()
            .Where(r => ids.Contains(r.Id) && r.QuranCircleId == circleId)
            .Select(r => r.StudentName)
            .ToList();

        return string.Join("، ", names.Where(n => !string.IsNullOrEmpty(n)));
    }

    private bool TryGetTeacherContext(out int teacherId, out int circleId)
    {
        teacherId = 0;
        circleId = 0;

        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var circleIdClaim = User.FindFirstValue("circleId");

        return int.TryParse(idClaim, out teacherId) && teacherId > 0
               && int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }
}
