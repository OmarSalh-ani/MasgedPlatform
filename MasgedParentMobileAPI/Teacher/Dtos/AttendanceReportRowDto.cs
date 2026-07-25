namespace MasgedTeacherMobileAPI.Dtos;

public class AttendanceReportRowDto
{
    public string StudentName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string DateFormatted { get; set; } = string.Empty;
    public bool IsPresent { get; set; }
    public string AttendanceText { get; set; } = string.Empty;
    public string DepartureTime { get; set; } = string.Empty;
    public string DepartureText { get; set; } = string.Empty;
}
