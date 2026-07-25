namespace MasgedTeacherMobileAPI.Dtos;

public class StudentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Group { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string IsPresentToday { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public int WarningCount { get; set; }
    public int ParentQuestionsCount { get; set; }
    public bool HasHealthCondition { get; set; }
    public bool HasLearningDifficulties { get; set; }
    public string DepartureStatusToday { get; set; } = string.Empty;
    public string DepartureTimeToday { get; set; } = string.Empty;
    public bool IsSpecial { get; set; }
    public bool IsElite { get; set; }
    public string PlanLevelName { get; set; } = string.Empty;
    public int? PlanLevelId { get; set; }
}
