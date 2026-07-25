namespace AdminAPI.DTOs.ParentsFollowup;

public class ParentsFollowupDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTime? Birthdate { get; set; }
    public string StudentGender { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? MaritalStatus { get; set; }
    public string? HealthCondition { get; set; }
    public string? HealthDetails { get; set; }
    public string? LearningDifficulties { get; set; }
    public string? LearningDifficultiesNotes { get; set; }
    public string? PhotoUrl { get; set; }
}
