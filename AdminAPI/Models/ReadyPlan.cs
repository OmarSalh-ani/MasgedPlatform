namespace AdminAPI.Models;

public class ReadyPlan
{
    public int Id { get; set; }
    public int PlanLevelId { get; set; }

    public virtual PlanLevel? PlanLevel { get; set; }
}
