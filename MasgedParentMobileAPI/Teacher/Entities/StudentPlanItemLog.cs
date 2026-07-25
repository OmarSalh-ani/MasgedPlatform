using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("StudentPlanItemLog")]
public class StudentPlanItemLog
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int PlanId { get; set; }

    [Required]
    [StringLength(50)]
    public string RowKey { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    public int TeacherId { get; set; }

    public DateTime LoggedAt { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
