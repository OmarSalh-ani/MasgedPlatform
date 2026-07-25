using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("StudentPlan")]
public class StudentPlan
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime PlanFromDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime PlanToDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsArchived { get; set; }
}
