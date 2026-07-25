namespace AdminAPI.DTOs.ParentPanelLogStatistics;

public class ParentPanelLogStatisticsResponseDto
{
    public string FromDate { get; set; } = string.Empty;
    public string ToDate { get; set; } = string.Empty;
    public ParentPanelLogStatisticsSummaryDto Summary { get; set; } = new();
    public List<ParentPanelLogEntryDto> Entries { get; set; } = [];
}
