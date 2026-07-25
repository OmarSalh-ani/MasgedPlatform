using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("TestHead")]
public class TestHead
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CircleId { get; set; }

    public int TeacherId { get; set; }

    [StringLength(500)]
    public string? TestFrom { get; set; }

    [StringLength(500)]
    public string? TestTo { get; set; }

    [StringLength(500)]
    public string? SurahName { get; set; }

    [StringLength(100)]
    public string? HezbNumber { get; set; }

    public DateTime TestDate { get; set; }

    public decimal FinalResult { get; set; }

    public decimal? MemorizationScore { get; set; }

    public decimal? TajweedScore { get; set; }

    public decimal? RevisionScore { get; set; }

    public decimal? TotalScore { get; set; }

    [StringLength(100)]
    public string? Grade { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(200)]
    public string? TestName { get; set; }

    [StringLength(100)]
    public string? TestType { get; set; }

    public virtual QuranCircle? QuranCircle { get; set; }

    public virtual RegisterForm? RegisterForm { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
