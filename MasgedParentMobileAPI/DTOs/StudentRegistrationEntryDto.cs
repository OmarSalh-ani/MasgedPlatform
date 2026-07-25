namespace MasgedParentMobileAPI.DTOs;

public class StudentRegistrationEntryDto
{
    public string FullName { get; set; } = string.Empty;
    public DateTime? Birthdate { get; set; }
    public int? Age { get; set; }
    public string? LearnCertificate { get; set; }
    public int WomanActivityTypeId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public bool HasHealthCondition { get; set; }
    public string? HealthDetails { get; set; }
    public bool HasLearningDifficulties { get; set; }
    public string? LearningDifficultiesDetails { get; set; }
}
