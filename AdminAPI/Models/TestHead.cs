namespace AdminAPI.Models;

public class TestHead
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CircleId { get; set; }
    public int TeacherId { get; set; }
    public string? TestFrom { get; set; }
    public string? TestTo { get; set; }
    public string? SurahName { get; set; }
    public string? HezbNumber { get; set; }
    public DateTime TestDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal FinalResult { get; set; }
    public decimal? MemorizationScore { get; set; }
    public decimal? TajweedScore { get; set; }
    public decimal? RevisionScore { get; set; }
    public decimal? TotalScore { get; set; }
    public string? Grade { get; set; }
    public string? Notes { get; set; }
    public string? TestName { get; set; }
    public string? TestType { get; set; }

    public virtual RegisterForm? Student { get; set; }
    public virtual QuranCircle? QuranCircle { get; set; }
    public virtual Teacher? Teacher { get; set; }
}
