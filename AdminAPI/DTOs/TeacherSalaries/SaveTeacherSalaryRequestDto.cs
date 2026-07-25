namespace AdminAPI.DTOs.TeacherSalaries;

public class SaveTeacherSalaryRequestDto
{
    public int TeacherId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BaseSalary { get; set; }
    public int DaysAttended { get; set; }
    public decimal TotalHours { get; set; }
    public decimal CalculatedSalary { get; set; }
    public string? Notes { get; set; }
    public DateTime? DayOffDate { get; set; }
}
