namespace AdminAPI.DTOs.AttendanceReport;

public class AttendanceReportListResponseDto
{
    public List<AttendanceReportRowDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public AttendanceReportSummaryDto Summary { get; set; } = new();
}
