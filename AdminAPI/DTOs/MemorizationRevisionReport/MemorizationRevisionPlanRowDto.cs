namespace AdminAPI.DTOs.MemorizationRevisionReport;

public class MemorizationRevisionPlanRowDto
{
    public string Status { get; set; } = string.Empty;
    public string SurahNameAr { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int FromAyah { get; set; }
    public int ToAyah { get; set; }
    public string PlanType { get; set; } = string.Empty;
}
