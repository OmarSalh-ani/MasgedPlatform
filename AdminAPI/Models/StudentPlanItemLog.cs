namespace AdminAPI.Models;

public class StudentPlanItemLog
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int PlanId { get; set; }
    public string RowKey { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public DateTime LoggedAt { get; set; }
}
