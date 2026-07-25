namespace MasgedTeacherMobileAPI.Dtos;

public class MemorizationRevisionReportResponseDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public List<PlanReportRowDto> Rows { get; set; } = [];
}
