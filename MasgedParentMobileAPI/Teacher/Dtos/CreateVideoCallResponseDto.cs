namespace MasgedTeacherMobileAPI.Dtos;

public class CreateVideoCallResponseDto
{
    public int Id { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public uint Uid { get; set; }
    public string MeetingName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
