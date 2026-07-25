namespace AdminAPI.Models;

public class StudentMemorizingCard
{
    public int Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? TheType { get; set; }
    public string? TestFrom { get; set; }
    public string? TestTo { get; set; }
    public string? IsDone { get; set; }
    public int StudentId { get; set; }
    public int CircleId { get; set; }
    public int TeacherId { get; set; }
    public string? Notes { get; set; }
    public string? ParentNotes { get; set; }
    public string? IsSaveDone { get; set; }
    public string? SurahName { get; set; }

    public virtual RegisterForm? Student { get; set; }
}
