namespace MasgedTeacherMobileAPI.Dtos;

public class CompletedSurahSummaryRowDto
{
    public string StudentName { get; set; } = string.Empty;
    public int SurahId { get; set; }
    public string SurahNameAr { get; set; } = string.Empty;
    public int FromAyah { get; set; }
    public int ToAyah { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
