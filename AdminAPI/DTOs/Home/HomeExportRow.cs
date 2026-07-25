namespace AdminAPI.DTOs.Home;

public class HomeExportRow
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public DateTime? Birthdate { get; set; }
    public int Age { get; set; }
    public string StudentGender { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string? FatherPhone2 { get; set; }
    public string? StudentPhone { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public string IsSpecial { get; set; } = string.Empty;
    public string WomanActivityType { get; set; } = string.Empty;
    public string? LearnCertificate { get; set; }
    public string? ThePassword { get; set; }
    public int LeaveCount { get; set; }
    public string CompleteFollowup { get; set; } = string.Empty;
    public bool IsElite { get; set; }
}
