namespace AdminAPI.DTOs.StudentPlan;

public class StudentPlanSurahOptionDto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
}

public class StudentPlanAyahDto
{
    public int AyahNumber { get; set; }
}

public class StudentPlanCircleOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class StudentPlanStudentOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? QuranCircleId { get; set; }
}

public class StudentPlanFormDataDto
{
    public List<StudentPlanCircleOptionDto> Circles { get; set; } = [];
    public List<StudentPlanStudentOptionDto> Students { get; set; } = [];
    public List<StudentPlanSurahOptionDto> Surahs { get; set; } = [];
    public List<string> MemorizationLevels { get; set; } = [];
    public List<string> PlanTypes { get; set; } = [];
    public bool CanModify { get; set; }
}
