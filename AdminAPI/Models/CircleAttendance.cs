namespace AdminAPI.Models;

public class CircleAttendance
{
    public int Id { get; set; }
    public int? CircleId { get; set; }
    public int StudentId { get; set; }
    public DateTime AttendanceDateTime { get; set; }
    public bool IsHere { get; set; }
    public int? TeacherId { get; set; }
    public DateTime? DepartureDate { get; set; }

    public virtual RegisterForm RegisterForm { get; set; } = null!;
}
