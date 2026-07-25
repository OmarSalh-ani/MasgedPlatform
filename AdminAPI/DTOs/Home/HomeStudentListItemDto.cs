namespace AdminAPI.DTOs.Home;

public class HomeStudentListItemDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string? FatherPhone2 { get; set; }
    public string? StudentPhone { get; set; }
    public string StudentGender { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? Birthdate { get; set; }
    public string? CreatedAt { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public int? QuranCircleId { get; set; }
    public int LeaveCount { get; set; }
    public string WomanActivityType { get; set; } = string.Empty;
    public string? LearnCertificate { get; set; }
    public string CompleteFollowup { get; set; } = "لا";
    public string IsSpecial { get; set; } = "لا";
    public string IsElite { get; set; } = "لا";
    public string? StudentImage { get; set; }
    public string PlanLevelName { get; set; } = "غير محدد";
}
