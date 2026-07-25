using System.ComponentModel.DataAnnotations.Schema;

namespace AdminAPI.Models;

public class StudentPlan
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime PlanFromDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime PlanToDate { get; set; }

    public DateTime? CreatedAt { get; set; }
    public bool IsArchived { get; set; }

    public virtual RegisterForm RegisterForm { get; set; } = null!;
    public virtual ICollection<StudentPlanMemorizing> StudentPlanMemorizings { get; set; } = [];
    public virtual ICollection<StudentPlanRevise> StudentPlanRevises { get; set; } = [];
    public virtual ICollection<StudentPlanItemLog> StudentPlanItemLogs { get; set; } = [];
}
