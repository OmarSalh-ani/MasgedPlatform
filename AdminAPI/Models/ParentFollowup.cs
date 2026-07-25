namespace AdminAPI.Models;

public class ParentFollowup
{
    public int StudentId { get; set; }
    public string? Address { get; set; }
    public string? MaritalStatus { get; set; }
    public string? HealthCondition { get; set; }
    public string? HealthDetails { get; set; }
    public string? LearningDifficulties { get; set; }
    public string? LearningDifficultiesNotes { get; set; }
    public string? PhotoPath { get; set; }

    public virtual RegisterForm? Student { get; set; }
}
