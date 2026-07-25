namespace AdminAPI.Models;

public class TeacherSalary
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
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
    public int? CreatedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedBy { get; set; }

    public Teacher? Teacher { get; set; }
}
