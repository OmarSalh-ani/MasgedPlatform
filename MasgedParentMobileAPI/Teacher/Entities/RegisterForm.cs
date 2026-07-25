using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("RegisterForm")]
public class RegisterForm
{
    public int Id { get; set; }

    [Required]
    [StringLength(300)]
    public string StudentName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? FullName { get; set; }

    [StringLength(300)]
    public string? FatherName { get; set; }

    public int Age { get; set; }

    [Required]
    [StringLength(50)]
    public string FatherPhone { get; set; } = string.Empty;

    [StringLength(50)]
    public string? FatherPhone2 { get; set; }

    [Required]
    [StringLength(5)]
    public string StudentGender { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public DateTime? Birthdate { get; set; }

    public int? QuranCircleId { get; set; }

    public bool IsSpecial { get; set; }

    public bool IsElite { get; set; }

    public int? PlanLevelId { get; set; }

    public virtual QuranCircle? QuranCircle { get; set; }

    public virtual ParentFollowup? ParentFollowup { get; set; }

    public virtual PlanLevel? PlanLevel { get; set; }

    public virtual ICollection<CircleAttendance> CircleAttendances { get; set; } = new List<CircleAttendance>();
}
