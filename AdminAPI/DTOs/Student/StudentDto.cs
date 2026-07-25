namespace AdminAPI.DTOs.Student;



public class StudentDto

{

    public int Id { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string FatherPhone { get; set; } = string.Empty;

    public string? AlternativePhone { get; set; }

    public string? ParentPanelPassword { get; set; }

    public int Age { get; set; }

    public string StudentGender { get; set; } = string.Empty;

    public int? QuranCircleId { get; set; }

    public int? PlanLevelId { get; set; }

    public bool IsSpecial { get; set; }

    public DateTime? CreatedAt { get; set; }

}

