using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("ParentFollowup")]
public class ParentFollowup
{
    [Key]
    public int StudentId { get; set; }

    [StringLength(500)]
    public string? address { get; set; }

    [StringLength(500)]
    public string? maritalStatus { get; set; }

    [StringLength(500)]
    public string? healthCondition { get; set; }

    [StringLength(500)]
    public string? healthDetails { get; set; }

    [StringLength(500)]
    public string? learningDifficulties { get; set; }

    [StringLength(500)]
    public string? learningDifficultiesNotes { get; set; }

    [StringLength(500)]
    public string? photoPath { get; set; }

    public virtual RegisterForm RegisterForm { get; set; } = null!;
}
