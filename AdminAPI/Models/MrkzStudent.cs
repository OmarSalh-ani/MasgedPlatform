namespace AdminAPI.Models;

public class MrkzStudent
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string FatherName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string? FatherPhone2 { get; set; }
    public string? StudentPhone { get; set; }
    public string StudentGender { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime? Birthdate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? QuranCircleId { get; set; }
    public int? WomanActivityType { get; set; }
    public string? LearnCertificate { get; set; }
    public string? ThePassword { get; set; }
    public bool IsSpecial { get; set; }
    public bool IsElite { get; set; }
    public int? PlanLevelId { get; set; }
    public int? IsGirl { get; set; }

    public virtual QuranCircle? QuranCircle { get; set; }
    public virtual WomanActivity? WomanActivity { get; set; }
    public virtual PlanLevel? PlanLevel { get; set; }
}
