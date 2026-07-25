namespace AdminAPI.DTOs.ParentPanelLogStatistics;

public class ParentPanelLogEntryDto
{
    public string ParentMobile { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string AccessDate { get; set; } = string.Empty;
    public string AccessTime { get; set; } = string.Empty;
}
