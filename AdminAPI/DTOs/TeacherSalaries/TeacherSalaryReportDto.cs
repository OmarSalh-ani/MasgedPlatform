namespace AdminAPI.DTOs.TeacherSalaries;

public class TeacherSalaryReportDto
{
    public TeacherSalaryReportSummaryDto Summary { get; set; } = new();
    public List<TeacherSalaryReportItemDto> Items { get; set; } = [];
}

public class TeacherSalaryReportSummaryDto
{
    public int TotalTeachers { get; set; }
    public decimal TotalSalary { get; set; }
    public decimal AverageSalary { get; set; }
    public int FullAttendance { get; set; }
    public int WithDeductions { get; set; }
}

public class TeacherSalaryReportItemDto
{
    public int Id { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int DaysAttended { get; set; }
    public decimal TotalHours { get; set; }
    public decimal? BaseSalary { get; set; }
    public decimal CalculatedSalary { get; set; }
    public decimal Deduction { get; set; }
}
