namespace AdminAPI.DTOs.TeachersAttendance;

public class TeachersAttendanceFilterOptionsDto
{
    public List<TeachersAttendanceTeacherOptionDto> Teachers { get; set; } = [];
}

public class TeachersAttendanceTeacherOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TeachersAttendanceListResponseDto
{
    public string FromDate { get; set; } = string.Empty;
    public string ToDate { get; set; } = string.Empty;
    public List<TeachersAttendanceRowDto> Items { get; set; } = [];
}

public class TeachersAttendanceRowDto
{
    public string TeacherName { get; set; } = string.Empty;
    public string AttendanceDateTime { get; set; } = string.Empty;
    public string? DepartureDateTime { get; set; }
    public decimal HoursWorked { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusClass { get; set; } = string.Empty;
}
