namespace MasgedTeacherMobileAPI.Dtos;

public class SpecialStudentsReportResponseDto
{
    public bool IsElite { get; set; }
    public string ReportTitle { get; set; } = string.Empty;
    public bool HasStudents { get; set; }
    public List<SpecialStudentReportDto> Students { get; set; } = [];
}
