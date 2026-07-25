namespace AdminAPI.DTOs.SpecialStudentsReport;

public class SpecialStudentsReportResponseDto
{
    public List<SpecialStudentsReportItemDto> Items { get; set; } = [];
    public SpecialStudentsReportStatsDto Stats { get; set; } = new();
}
