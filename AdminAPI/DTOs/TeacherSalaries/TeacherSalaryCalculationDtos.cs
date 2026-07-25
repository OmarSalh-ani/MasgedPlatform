namespace AdminAPI.DTOs.TeacherSalaries;

public class CalculateTeacherAttendanceRequestDto
{
    public int TeacherId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public class CalculateTeacherSalaryRequestDto
{
    public int TeacherId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BaseSalary { get; set; }
    public DateTime? DayOffDate { get; set; }
}

public class DailyAttendanceDetailDto
{
    public string Date { get; set; } = string.Empty;
    public string DateFormatted { get; set; } = string.Empty;
    public string AttendanceTime { get; set; } = string.Empty;
    public string DepartureTime { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public bool IsValid { get; set; }
}

public class AttendanceCalculationResultDto
{
    public int DaysAttended { get; set; }
    public decimal TotalHours { get; set; }
    public List<DailyAttendanceDetailDto> DailyDetails { get; set; } = [];
}

public class SalaryCalculationResultDto
{
    public int DaysAttended { get; set; }
    public decimal TotalHours { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal CalculatedSalary { get; set; }
    public decimal Deduction { get; set; }
    public int RequiredDays { get; set; }
    public List<DailyAttendanceDetailDto> DailyDetails { get; set; } = [];
}

public class AutoCalculateMonthRequestDto
{
    public int Month { get; set; }
    public int Year { get; set; }
}

public class AutoCalculateMonthResultDto
{
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class PaySelectedSalariesRequestDto
{
    public List<int> SalaryIds { get; set; } = [];
}

public class PaySelectedSalariesResultDto
{
    public int ExpensesCreated { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
}
