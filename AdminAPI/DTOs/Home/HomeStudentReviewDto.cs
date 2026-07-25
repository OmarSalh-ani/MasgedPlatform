namespace AdminAPI.DTOs.Home;

public class HomeStudentReviewDto
{
    public string ReviewType { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string TestFrom { get; set; } = string.Empty;
    public string TestTo { get; set; } = string.Empty;
    public string SurahName { get; set; } = string.Empty;
    public string IsDone { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string ParentNotes { get; set; } = string.Empty;
    public string IsSaveDone { get; set; } = string.Empty;
    public string DisplayNotes { get; set; } = string.Empty;
}
