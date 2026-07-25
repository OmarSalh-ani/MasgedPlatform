namespace MasgedParentMobileAPI.DTOs;

public sealed class ParentScheduleSlotDto
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string CircleName { get; set; } = string.Empty;

    public List<string> WeekdaysArabic { get; set; } = [];
}
