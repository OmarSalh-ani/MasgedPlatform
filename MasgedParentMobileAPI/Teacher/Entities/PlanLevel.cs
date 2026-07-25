using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("PlanLevel")]
public class PlanLevel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string LevelName { get; set; } = string.Empty;

    public byte UnitType { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedByTeacherId { get; set; }
}
