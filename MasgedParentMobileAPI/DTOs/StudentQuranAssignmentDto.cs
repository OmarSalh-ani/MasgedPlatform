namespace MasgedParentMobileAPI.DTOs;

public sealed class StudentQuranAssignmentDto
{
    public int MemorizeSurahId { get; set; }

    public string MemorizeSurahNameArabic { get; set; } = string.Empty;

    public int MemorizeFromAyah { get; set; }

    public int MemorizeToAyah { get; set; }

    public int? ReviseSurahId { get; set; }

    public string? ReviseSurahNameArabic { get; set; }

    public int ReviseFromAyah { get; set; }

    public int ReviseToAyah { get; set; }
}
