namespace AdminAPI.DTOs.Statistics;

public class StatisticsResponseDto
{
    public CircleStatisticsDto CircleStatistics { get; set; } = new();
    public AdditionalStatisticsDto AdditionalStatistics { get; set; } = new();
}
