namespace MasgedParentMobileAPI.DTOs;

public sealed class ParentVideoCallJoinDto
{
    public int MeetingId { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public uint Uid { get; set; }
    public string MeetingName { get; set; } = string.Empty;
    public int StudentId { get; set; }

    /// <summary>Teacher Agora camera uid (DB teacher id). Screen share uses this + 1.</summary>
    public uint TeacherRtcUid { get; set; }
}
