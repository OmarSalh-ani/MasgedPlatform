namespace AdminAPI.DTOs.TeacherSalaries;

public class TeacherSalaryListItemDto
{
    public int Id { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int DaysAttended { get; set; }
    public decimal TotalHours { get; set; }
    public decimal? BaseSalary { get; set; }
    public decimal CalculatedSalary { get; set; }
    public DateTime? DayOffDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
