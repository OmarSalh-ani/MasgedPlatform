namespace AdminAPI.DTOs.CurrentStudentsPlans;

public class CurrentStudentPlanStudentLookupFiltersDto
{
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
