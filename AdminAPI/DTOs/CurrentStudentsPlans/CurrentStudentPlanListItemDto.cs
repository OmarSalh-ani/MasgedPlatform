namespace AdminAPI.DTOs.CurrentStudentsPlans;

public class CurrentStudentPlanListItemDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int TotalDays { get; set; }
    public int ElapsedDays { get; set; }
    public int RemainingDays { get; set; }
    public string CircleName { get; set; } = string.Empty;
}
