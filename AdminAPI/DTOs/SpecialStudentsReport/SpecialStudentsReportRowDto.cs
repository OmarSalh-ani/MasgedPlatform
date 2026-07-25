namespace AdminAPI.DTOs.SpecialStudentsReport;

public class SpecialStudentsReportRowDto
{
    public string StudentName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string? FatherPhone2 { get; set; }
    public string? StudentPhone { get; set; }
    public string StudentGender { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? PhotoPath { get; set; }
    public int? CircleId { get; set; }
}
