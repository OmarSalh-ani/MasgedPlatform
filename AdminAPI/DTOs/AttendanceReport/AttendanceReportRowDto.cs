namespace AdminAPI.DTOs.AttendanceReport;

public class AttendanceReportRowDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string? FatherPhone { get; set; }
    public string Date { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public bool IsPresent { get; set; }
    public bool IsDeparted { get; set; }
    public string? DepartureTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
