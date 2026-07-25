using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("StudentMemorizingCard")]
public class StudentMemorizingCard
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    [StringLength(100)]
    public string TheType { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string DayName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string TestFrom { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string TestTo { get; set; } = string.Empty;

    [StringLength(500)]
    public string? SurahName { get; set; }

    [Required]
    [StringLength(100)]
    public string IsDone { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public int CircleId { get; set; }

    public int TeacherId { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(500)]
    public string? ParentNotes { get; set; }

    [StringLength(100)]
    public string? IsSaveDone { get; set; }

    public virtual RegisterForm? RegisterForm { get; set; }
}
