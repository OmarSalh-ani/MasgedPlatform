namespace AdminAPI.DTOs.ParentsFollowup;

public class SaveParentsFollowupRequestDto
{
    public string StudentName { get; set; } = string.Empty;
    public DateTime? Birthdate { get; set; }
    public string StudentGender { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string HealthCondition { get; set; } = string.Empty;
    public string? HealthDetails { get; set; }
    public string LearningDifficulties { get; set; } = string.Empty;
    public string? LearningDifficultiesNotes { get; set; }
    public IFormFile? Photo { get; set; }
}
