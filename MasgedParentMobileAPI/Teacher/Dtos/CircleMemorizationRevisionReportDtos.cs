namespace MasgedTeacherMobileAPI.Dtos;

public class CircleMemorizationRevisionReportRowDto
{
    public int Sequence { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string NewMemorization { get; set; } = string.Empty;
    public string Revision { get; set; } = string.Empty;
}

public class CircleMemorizationRevisionReportMetaDto
{
    public string MosqueName { get; set; } = "مسجد الشيخ مبارك عبد الله المبارك الصباح";
    public string CircleName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public DateTime PrintedAt { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<CircleMemorizationRevisionReportRowDto> Rows { get; set; } = [];
}
