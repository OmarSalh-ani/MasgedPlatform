namespace MasgedTeacherMobileAPI.Dtos;

public class CreateVideoCallRequestDto
{
    public string? MeetingName { get; set; }

    public string? TeacherName { get; set; }

    public DateTime? StartDateTime { get; set; }

    public List<int> StudentIds { get; set; } = [];

    public bool SendWhatsApp { get; set; } = true;
}
