namespace AdminAPI.DTOs.MemorizationRevisionReport;

public class MemorizationRevisionReportResponseDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public List<MemorizationRevisionPlanRowDto> Rows { get; set; } = [];
}
