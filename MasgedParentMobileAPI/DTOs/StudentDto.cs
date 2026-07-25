namespace MasgedParentMobileAPI.DTOs;

public class StudentDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Status { get; set; } = "absent";
    public string? AttendTime { get; set; }
    public string? DepartureTime { get; set; }
    public string? LogTime { get; set; }
    public string? Notes { get; set; }
    public int AttendancePercent { get; set; }
    public string NextSession { get; set; } = string.Empty;
    public Dictionary<string, bool?> WeeklyAttendance { get; set; } = new();

    /// <summary>Teacher of the student's Quran circle, if assigned.</summary>
    public int? TeacherId { get; set; }

    public string? TeacherName { get; set; }
}
