namespace MasgedTeacherMobileAPI.Dtos;

public class VideoCallListItemDto
{
    public int Id { get; set; }
    public string MeetingName { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    /// <summary>Agora channel name (stored in MeetingsInfo.MeetingUrl).</summary>
    public string ChannelName { get; set; } = string.Empty;
    public string? StudentIds { get; set; }
    public string StudentNames { get; set; } = string.Empty;
    public byte Status { get; set; }
    public DateTime? EndedAt { get; set; }
    public string? TeacherNotes { get; set; }
}
