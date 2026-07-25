namespace MasgedTeacherMobileAPI.Dtos;

public class TeacherAttendanceStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? AttendanceTime { get; set; }
    public string? DepartureTime { get; set; }
    public bool HasFingerprintRegistered { get; set; }
}
