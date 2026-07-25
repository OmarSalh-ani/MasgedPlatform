namespace MasgedParentMobileAPI.DTOs;

public class UpdateStudentRequest
{
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public string? ParentName { get; set; }
    public string? Phone { get; set; }
    public string? MaritalStatus { get; set; }
    public bool? HasHealthCondition { get; set; }
    public string? HealthDetails { get; set; }
    public bool? HasLearningDifficulties { get; set; }
    public string? LearningDifficultiesDetails { get; set; }
}
