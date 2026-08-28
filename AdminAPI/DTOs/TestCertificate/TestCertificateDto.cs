namespace AdminAPI.DTOs.TestCertificate;

public class TestCertificateDto
{
    public int TestId { get; set; }

    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string TestDate { get; set; } = string.Empty;
    public string TestFrom { get; set; } = string.Empty;
    public string TestTo { get; set; } = string.Empty;
    public decimal MemorizationScore { get; set; }
    public decimal TajweedScore { get; set; }
    public decimal RevisionScore { get; set; }
}
