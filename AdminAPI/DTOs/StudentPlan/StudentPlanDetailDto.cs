namespace AdminAPI.DTOs.StudentPlan;

public class StudentPlanListOptionDto
{
    public int Id { get; set; }
    public string Display { get; set; } = string.Empty;
}

public class StudentPlanItemDto
{
    public string Key { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public string MemorizationLevel { get; set; } = string.Empty;
    public int SurahId { get; set; }
    public string SurahName { get; set; } = string.Empty;
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public string PlanDateFormatted { get; set; } = string.Empty;
}

public class StudentPlanHeaderDto
{
    public string MemorizationLevel { get; set; } = "محدد الحفظ";
    public string PlanStartDate { get; set; } = string.Empty;
    public string PlanEndDate { get; set; } = string.Empty;
}

public class StudentPlanDetailDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public List<StudentPlanListOptionDto> Plans { get; set; } = [];
    public StudentPlanHeaderDto Header { get; set; } = new();
    public List<StudentPlanItemDto> Items { get; set; } = [];
    public bool CanModify { get; set; }
}

public class StudentPlanResolveDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int? PlanId { get; set; }
    public bool ShouldCreateNew { get; set; }
}

public class StudentPlanEditPrefillDto
{
    public string MemorizationLevel { get; set; } = string.Empty;
    public string PlanStartDate { get; set; } = string.Empty;
    public string PlanEndDate { get; set; } = string.Empty;
    public int SurahId { get; set; }
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public int? PlanId { get; set; }
}
