namespace AdminAPI.DTOs.AttendanceReport;

public class SaveDepartureResultDto
{
    public string Message { get; set; } = string.Empty;
    public int SavedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
}
