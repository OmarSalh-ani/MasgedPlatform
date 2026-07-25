namespace MasgedTeacherMobileAPI.Dtos;

public class AttendanceReportResponseDto
{
    public List<AttendanceReportRowDto> Data { get; set; } = [];
    public AttendanceReportSummaryDto Summary { get; set; } = new();
}
