using MasgedParentMobileAPI.Hubs;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Enums;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Services;

public interface IVideoCallTerminationService
{
    /// <summary>
    /// Marks the meeting ended, clears hub runtime state, and notifies all clients to leave Agora.
    /// </summary>
    Task<bool> TerminateMeetingAsync(
        int meetingId,
        int teacherId,
        string? teacherNotes,
        CancellationToken cancellationToken = default);
}

internal sealed class VideoCallTerminationService(
    AppDbContext db,
    IHubContext<VideoCallHub> hubContext) : IVideoCallTerminationService
{
    public async Task<bool> TerminateMeetingAsync(
        int meetingId,
        int teacherId,
        string? teacherNotes,
        CancellationToken cancellationToken = default)
    {
        var meeting = await db.MeetingsInfo
            .FirstOrDefaultAsync(
                m => m.Id == meetingId && m.TeacherId == teacherId,
                cancellationToken);

        if (meeting is null)
            return false;

        if (meeting.Status != MeetingStatus.Ended)
        {
            meeting.Status = MeetingStatus.Ended;
            meeting.EndedAt = KuwaitTime.Now;
            if (!string.IsNullOrWhiteSpace(teacherNotes))
                meeting.TeacherNotes = teacherNotes.Trim();

            await db.SaveChangesAsync(cancellationToken);
        }

        VideoCallMeetingRuntimeState.Remove(meetingId);

        await hubContext.Clients
            .Group(VideoCallHubGroups.Meeting(meetingId))
            .SendAsync("CallEnded", meetingId, cancellationToken);

        return true;
    }
}

internal static class VideoCallHubGroups
{
    public static string Meeting(int meetingId) => $"videocall_{meetingId}";
}
