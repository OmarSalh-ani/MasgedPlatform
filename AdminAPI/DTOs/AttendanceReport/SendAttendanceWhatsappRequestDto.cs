namespace AdminAPI.DTOs.AttendanceReport;

public class SendAttendanceWhatsappRequestDto
{
    public List<int> StudentIds { get; set; } = [];
    public string Message { get; set; } = string.Empty;
}
