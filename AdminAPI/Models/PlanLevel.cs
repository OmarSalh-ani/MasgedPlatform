namespace AdminAPI.Models;

public class PlanLevel
{
    public int Id { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public byte UnitType { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedByTeacherId { get; set; }

    public virtual ICollection<RegisterForm> RegisterForms { get; set; } = [];
    public virtual ICollection<ReadyPlan> ReadyPlans { get; set; } = [];
}
