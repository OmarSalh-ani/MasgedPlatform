namespace MasgedTeacherMobileAPI.Dtos;

public class AddStudentsToVideoCallRequestDto
{
    public List<int> StudentIds { get; set; } = [];

    public bool SendWhatsApp { get; set; } = true;
}
