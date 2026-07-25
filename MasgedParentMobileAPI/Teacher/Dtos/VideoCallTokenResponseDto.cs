namespace MasgedTeacherMobileAPI.Dtos;

public class VideoCallTokenResponseDto
{
    public string ChannelName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public uint Uid { get; set; }
}
