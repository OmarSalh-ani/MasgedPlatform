using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/parent/VideoCall")]
public sealed class ParentVideoCallController(
    NewMasgedTeacherAPIDBContext db,
    AgoraTokenService agoraTokens) : ControllerBase
{
    /// <summary>Active video call for a teacher + student (parent chat join).</summary>
    [HttpGet("active")]
    public async Task<ActionResult<ParentActiveVideoCallDto>> GetActive(
        [FromQuery] int teacherId,
        [FromQuery] int studentId,
        CancellationToken cancellationToken)
    {
        if (teacherId <= 0 || studentId <= 0)
            return BadRequest();

        var fp = User.FindFirstValue("fatherPhone");
        if (string.IsNullOrEmpty(fp))
            return Unauthorized();

        var variants = PhoneNormalizer.GetVariants(fp).ToList();
        var parentStudentIds = (await db.RegisterForms
            .Where(r =>
                variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken)).ToHashSet();

        if (!parentStudentIds.Contains(studentId))
            return Forbid();

        var candidates = await db.MeetingsInfos.AsNoTracking()
            .Where(m => m.TeacherId == teacherId && m.Status == 0)
            .OrderByDescending(m => m.CreatedAt)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var meeting in candidates)
        {
            var channel = (meeting.MeetingUrl ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(channel)
                || channel == "_pending"
                || !channel.StartsWith("masged_", StringComparison.OrdinalIgnoreCase))
                continue;

            if (string.IsNullOrWhiteSpace(meeting.StudentIds))
                continue;

            var invited = meeting.StudentIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, out var sid) ? sid : (int?)null)
                .Where(s => s.HasValue)
                .Select(s => s!.Value);

            if (!invited.Contains(studentId))
                continue;

            return Ok(new ParentActiveVideoCallDto { MeetingId = meeting.Id });
        }

        return NotFound();
    }

    /// <summary>Join an Agora channel for a scheduled video call tied to parent's children.</summary>
    [HttpGet("{id:int}/join")]
    public async Task<ActionResult<ParentVideoCallJoinDto>> Join(int id,
        [FromQuery] int? studentId,
        CancellationToken cancellationToken)
    {
        var fp = User.FindFirstValue("fatherPhone");
        if (string.IsNullOrEmpty(fp))
            return Unauthorized();

        var variants = PhoneNormalizer.GetVariants(fp).ToList();

        var parentStudentIds = (await db.RegisterForms
            .Where(r =>
                variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken)).ToHashSet();

        var meeting = await db.MeetingsInfos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (meeting is null)
            return NotFound();

        if (meeting.Status == 1)
            return BadRequest("انتهت هذه المكالمة ولا يمكن الانضمام إليها.");

        var channel = (meeting.MeetingUrl ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(channel)
            || channel == "_pending"
            || !channel.StartsWith("masged_", StringComparison.OrdinalIgnoreCase))
            return BadRequest("هذه الدعوة قديمة أو غير صالحة. اطلب من المعلم إنشاء مكالمة جديدة.");

        if (string.IsNullOrWhiteSpace(meeting.StudentIds))
            return Forbid();

        var invited = meeting.StudentIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => int.TryParse(s, out var sid) ? sid : (int?)null)
            .Where(s => s.HasValue)
            .Select(s => s!.Value)
            .Where(parentStudentIds.Contains)
            .ToList();

        if (invited.Count == 0)
            return Forbid();

        var chosenSid = studentId is > 0 && invited.Contains(studentId.Value)
            ? studentId.Value
            : invited[0];

        var uid = VideoCallUidRules.ParentUid(chosenSid);
        string token;
        try
        {
            token = agoraTokens.BuildRtcPublisherToken(channel, uid);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok(new ParentVideoCallJoinDto
        {
            MeetingId = meeting.Id,
            ChannelName = channel,
            Token = token,
            Uid = uid,
            MeetingName = string.IsNullOrWhiteSpace(meeting.MeetingName)
                ? "مكالمة فيديو"
                : meeting.MeetingName ?? "مكالمة فيديو",
            StudentId = chosenSid,
            TeacherRtcUid = VideoCallUidRules.TeacherUid(meeting.TeacherId),
        });
    }
}
