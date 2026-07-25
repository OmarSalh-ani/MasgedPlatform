namespace AdminAPI.DTOs.Student;



public class SaveStudentRequestDto

{

    public string FullName { get; set; } = string.Empty;

    public string FatherPhone { get; set; } = string.Empty;

    public string? AlternativePhone { get; set; }

    public string? ParentPanelPassword { get; set; }

    public int? Age { get; set; }

    public string StudentGender { get; set; } = string.Empty;

    public int? QuranCircleId { get; set; }

    public int? PlanLevelId { get; set; }

    public bool IsSpecial { get; set; }

}

