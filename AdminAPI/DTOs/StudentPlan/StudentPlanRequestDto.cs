namespace AdminAPI.DTOs.StudentPlan;

public class CreateStudentPlanRequestDto
{
    public int StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class PlanRowInputDto
{
    public int SurahId { get; set; }
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public string PlanType { get; set; } = "حفظ";
}

public class EditPlanRowInputDto
{
    public string Key { get; set; } = string.Empty;
    public int SurahId { get; set; }
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public string PlanType { get; set; } = "حفظ";
}

public class SaveStudentPlanRequestDto
{
    public List<int> StudentIds { get; set; } = [];
    public int? StudentId { get; set; }
    public int? PlanId { get; set; }
    public string MemorizationLevel { get; set; } = "محدد الحفظ";
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanEndDate { get; set; }
    public bool EditMode { get; set; }
    public List<EditPlanRowInputDto> EditRows { get; set; } = [];
    public List<PlanRowInputDto> NewRows { get; set; } = [];
}

public class UpdateStudentPlanItemRequestDto
{
    public string EditKey { get; set; } = string.Empty;
    public string MemorizationLevel { get; set; } = "محدد الحفظ";
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanEndDate { get; set; }
    public int SurahId { get; set; }
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public string PlanType { get; set; } = "حفظ";
}

public class CreateStudentPlanResponseDto
{
    public int PlanId { get; set; }
}
