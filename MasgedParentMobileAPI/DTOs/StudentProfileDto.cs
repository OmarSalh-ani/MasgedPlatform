namespace MasgedParentMobileAPI.DTOs;

public class StudentProfileDto : StudentDto
{
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public string? ParentName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ParentMaritalStatus { get; set; }
    public bool HasHealthCondition { get; set; }
    public string? HealthConditionDetails { get; set; }
    public bool HasLearningDifficulties { get; set; }
    public string? LearningDifficultiesDetails { get; set; }
    public string? MemorizationProgress { get; set; }
    public string? RevisionProgress { get; set; }
    public int AbsentDaysThisMonth { get; set; }
    public int LateCount { get; set; }
}
