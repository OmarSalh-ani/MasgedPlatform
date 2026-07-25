namespace MasgedTeacherMobileAPI.Dtos;

public class TestCertificateDto
{
    public int TestId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string TestPeriod { get; set; } = "الفصل الأول";
    public List<string> HizbCells { get; set; } = [];
    public string MemorizationScore { get; set; } = string.Empty;
    public string TajweedScore { get; set; } = string.Empty;
    public string RevisionScore { get; set; } = string.Empty;
    public string TotalScore { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string TestDate { get; set; } = string.Empty;
}
