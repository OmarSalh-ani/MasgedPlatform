namespace MasgedTeacherMobileAPI.Dtos;

public class SpecialStudentReportDto
{
    public string StudentName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string BadgeText { get; set; } = string.Empty;
    public bool IsElite { get; set; }
}
