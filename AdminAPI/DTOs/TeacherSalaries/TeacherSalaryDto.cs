namespace AdminAPI.DTOs.TeacherSalaries;

public class TeacherSalaryDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal? BaseSalary { get; set; }
    public int DaysAttended { get; set; }
    public decimal TotalHours { get; set; }
    public DateTime? DayOffDate { get; set; }
    public decimal CalculatedSalary { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
