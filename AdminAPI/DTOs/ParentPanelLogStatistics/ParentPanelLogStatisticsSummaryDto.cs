namespace AdminAPI.DTOs.ParentPanelLogStatistics;

public class ParentPanelLogStatisticsSummaryDto
{
    public int ParentsOpened { get; set; }
    public int ParentsNotOpened { get; set; }
    public int TotalLogEntries { get; set; }
    public string Percentage { get; set; } = "0%";
}
