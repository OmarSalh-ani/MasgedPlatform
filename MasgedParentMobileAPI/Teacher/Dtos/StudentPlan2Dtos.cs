namespace MasgedTeacherMobileAPI.Dtos;

public class StudentPlan2StudentListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class StudentPlan2StudentOverviewDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public bool IsNewPlanMode { get; set; }
    public int? SuggestedPlanId { get; set; }
    public List<StudentPlanSummaryDto> Plans { get; set; } = [];
}

public class StudentPlanSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime PlanFromDate { get; set; }
    public DateTime PlanToDate { get; set; }
    public bool IsCurrent { get; set; }
}

public class StudentPlan2DetailDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateTime PlanFromDate { get; set; }
    public DateTime PlanToDate { get; set; }
    public string? MemorizationLevel { get; set; }
    public PlanProgressDto Progress { get; set; } = new();
    public PlanRowDto? CurrentMemorizing { get; set; }
    public PlanRowDto? CurrentRevise { get; set; }
    public List<PlanRowDto> TathbitRows { get; set; } = [];
    public List<PlanRowDto> AllRows { get; set; } = [];
    public List<PlanRowDto> EditableMemorizingRows { get; set; } = [];
    public List<CalendarDayDto> CalendarDays { get; set; } = [];
    public List<AssessmentLogEntryDto> AssessmentLog { get; set; } = [];
    public List<StudentPlanSummaryDto> Plans { get; set; } = [];
}

public class PlanProgressDto
{
    public int Passed { get; set; }
    public int Failed { get; set; }
    public int Pending { get; set; }
    public int Retake { get; set; }
    public int Total { get; set; }
    public int DaysRemaining { get; set; }
    public int TotalPlanDays { get; set; }
    public int DaysElapsed { get; set; }
    public int ProgressPercent { get; set; }
    public int CircleDaysInRange { get; set; }
}

public class PlanRowDto
{
    public string Key { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public string MemorizationLevel { get; set; } = string.Empty;
    public int SurahId { get; set; }
    public string SurahName { get; set; } = string.Empty;
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public DateTime PlanDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplay { get; set; } = string.Empty;
    public DateTime? MemorizeDate { get; set; }
    public DateTime? ReviseDate { get; set; }
    public bool IsManual { get; set; }
}

public class CalendarDayDto
{
    public DateTime Date { get; set; }
    public string DayNameAr { get; set; } = string.Empty;
    public bool IsCircleDay { get; set; }
    public List<CalendarSurahItemDto> Items { get; set; } = [];
}

public class CalendarSurahItemDto
{
    public int SurahId { get; set; }
    public string SurahName { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
}

public class AssessmentLogEntryDto
{
    public string RowLabel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusDisplay { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string LoggedAtFormatted { get; set; } = string.Empty;
}

public class SavePlanRowsRequestDto
{
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanEndDate { get; set; }
    public string? PlanName { get; set; }
    public List<PlanRowInputDto> Rows { get; set; } = [];
}

public class BulkAssignPlanRequestDto
{
    public List<int> StudentIds { get; set; } = [];
    public bool AddToExistingPlan { get; set; }
    public SavePlanRowsRequestDto Plan { get; set; } = new();
}

public class BulkAssignPlanResponseDto
{
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkAssignPlanStudentResultDto> Results { get; set; } = [];
}

public class BulkAssignPlanStudentResultDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int? PlanId { get; set; }
    public string? Message { get; set; }
}

public class PlanRowInputDto
{
    public int SurahId { get; set; }
    public string? SurahName { get; set; }
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public string PlanType { get; set; } = "حفظ";
    /// <summary>Optional per-row schedule date. Ignored when <see cref="UseNextWorkDay"/> is true.</summary>
    public DateTime? PlanDate { get; set; }
    /// <summary>Optional status; defaults to منتظر التسميع when omitted.</summary>
    public string? Status { get; set; }
    /// <summary>When true, server sets PlanDate to the next circle work day after plan start / today.</summary>
    public bool UseNextWorkDay { get; set; }
}

public class SaveReviseRowsRequestDto
{
    public string ReviseDate { get; set; } = string.Empty;
    public List<PlanRowInputDto> Rows { get; set; } = [];
}

public class AssignReviseToPlanRequestDto
{
    public int PlanLevelId { get; set; }
    public int FromSurahId { get; set; }
    public int ToSurahId { get; set; }
    public int? FromJozz { get; set; }
    public int? ToJozz { get; set; }
    public string FromDate { get; set; } = string.Empty;
    public string ToDate { get; set; } = string.Empty;
    public int? FromAyahNumber { get; set; }
    public int? ToAyahNumber { get; set; }
}

public class LogPlanRowStatusRequestDto
{
    public string RowKey { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TabType { get; set; }
    /// <summary>Optional confirmed end ayah when marking pass; if less than current to-ayah, remainder is scheduled next work day.</summary>
    public int? ConfirmedToAyahNumber { get; set; }
}

public class LogPlanRowStatusResponseDto
{
    public PlanProgressDto Progress { get; set; } = new();
    public AssessmentLogEntryDto? LogRow { get; set; }
    public PlanRowDto? NextReviseRecord { get; set; }
}

public class SaveNextDateRequestDto
{
    public List<string> ItemKeys { get; set; } = [];
    public string Date { get; set; } = string.Empty;
}

public class UpdatePlanRowRequestDto
{
    public int SurahId { get; set; }
    public string? SurahName { get; set; }
    public int FromAyahNumber { get; set; }
    public int ToAyahNumber { get; set; }
    public string? PlanType { get; set; }
}

public class UpdatePlanDatesRequestDto
{
    public DateTime PlanStartDate { get; set; }
    public DateTime PlanEndDate { get; set; }
}
