namespace MasgedTeacherMobileAPI.Dtos;

public class AvailableStudentDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public int Age { get; set; }
}
