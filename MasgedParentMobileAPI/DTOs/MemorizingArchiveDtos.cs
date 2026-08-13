namespace MasgedParentMobileAPI.DTOs;

public class MemorizingArchiveItemDto
{
    public int Id { get; set; }
    public string TheType { get; set; } = string.Empty;
    public string TestFrom { get; set; } = string.Empty;
    public string TestTo { get; set; } = string.Empty;
    public string SurahName { get; set; } = string.Empty;
    public string IsDone { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateJuzHizbReviewDto
{
    public string UnitType { get; set; } = string.Empty;
    public int Number { get; set; }
}
