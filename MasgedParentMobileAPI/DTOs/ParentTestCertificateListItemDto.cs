namespace MasgedParentMobileAPI.DTOs;

public sealed class ParentTestCertificateListItemDto
{
    public int TestId { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string TestDate { get; set; } = string.Empty;

    public string Grade { get; set; } = string.Empty;

    public string TotalScore { get; set; } = string.Empty;

    public string TestFrom { get; set; } = string.Empty;

    public string TestTo { get; set; } = string.Empty;
}
