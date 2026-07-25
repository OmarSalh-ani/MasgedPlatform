namespace MasgedTeacherMobileAPI.Dtos;

public class TeacherAttendanceLogItemDto
{
    public int Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Day { get; set; } = string.Empty;
    public string StatusKey { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AttendanceTime { get; set; } = string.Empty;
    public string? DepartureTime { get; set; }
}

public class TeacherAttendanceLogSummaryDto
{
    public int TotalRecords { get; set; }
    public int TotalWithDeparture { get; set; }
    public int TotalAttendanceOnly { get; set; }
}

public class TeacherAttendanceLogResponseDto
{
    public string FromDate { get; set; } = string.Empty;
    public string ToDate { get; set; } = string.Empty;
    public TeacherAttendanceLogSummaryDto Summary { get; set; } = new();
    public List<TeacherAttendanceLogItemDto> Records { get; set; } = [];
}
