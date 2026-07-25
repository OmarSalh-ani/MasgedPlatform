namespace MasgedParentMobileAPI.DTOs;

public class AttendanceMonthResponseDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<AttendanceRecordDto> Records { get; set; } = new();
}
