namespace MasgedTeacherMobileAPI.Dtos;

public class VideoCallStudentsResponseDto
{
    public string TeacherName { get; set; } = string.Empty;
    public List<VideoCallStudentDto> Students { get; set; } = [];
}
