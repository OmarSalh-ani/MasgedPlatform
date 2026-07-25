using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("StudentPlanRevise")]
public class StudentPlanRevise
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int PlanId { get; set; }

    [Required]
    [StringLength(50)]
    public string MemorizationLevel { get; set; } = string.Empty;

    public int SurahId { get; set; }

    public int FromAyahNumber { get; set; }

    public int ToAyahNumber { get; set; }

    [Column(TypeName = "date")]
    public DateTime PlanDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? PlanEndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    [Column(TypeName = "date")]
    public DateTime? MemorizeDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ReviseDate { get; set; }

    public virtual RegisterForm RegisterForm { get; set; } = null!;

    public virtual QuranSurah QuranSurah { get; set; } = null!;
}
