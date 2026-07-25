namespace MasgedParentMobileAPI.DTOs;

public class ParentStudentPlanOverviewDto
{
    public int? PlanId { get; set; }
    public string? PlanName { get; set; }
    public DateTime? PlanFromDate { get; set; }
    public DateTime? PlanToDate { get; set; }
    public string? MemorizationLevel { get; set; }
    public ParentPlanProgressDto Progress { get; set; } = new();
}

public class ParentPlanProgressDto
{
    public int Passed { get; set; }
    public int Failed { get; set; }
    public int Pending { get; set; }
    public int Total { get; set; }
    public int ProgressPercent { get; set; }
    public int DaysRemaining { get; set; }
    public int TotalPlanDays { get; set; }
}

public class ParentPlanRowDto
{
    public string SurahName { get; set; } = string.Empty;
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplay { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
}
