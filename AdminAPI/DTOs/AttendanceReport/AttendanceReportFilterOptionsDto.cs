namespace AdminAPI.DTOs.AttendanceReport;

public class AttendanceReportFilterOptionsDto
{
    public List<AttendanceReportLookupDto> Circles { get; set; } = [];
    public List<AttendanceReportLookupDto> Teachers { get; set; } = [];
}

public class AttendanceReportLookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
