using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("Teacher")]
public class Teacher
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Password { get; set; } = string.Empty;

    public bool UsersManage { get; set; }

    public bool? IsGirlTeacher { get; set; }

    /// <summary>SHA-256 hex hash for attendance/departure biometric verification (never raw biometric data).</summary>
    [StringLength(64)]
    public string? AttendanceFingerprintHash { get; set; }

    public virtual ICollection<QuranCircle> QuranCircles { get; set; } = new List<QuranCircle>();
}
