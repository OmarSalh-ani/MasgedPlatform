namespace MasgedTeacherMobileAPI.Dtos;

public class ReadyPlanDto
{
    public int Id { get; set; }
    public int PlanLevelId { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public string FromSurahName { get; set; } = string.Empty;
    public string ToSurahName { get; set; } = string.Empty;
    public int FromSurahId { get; set; }
    public int ToSurahId { get; set; }
    public int? FromAyah { get; set; }
    public int? ToAyah { get; set; }
    public int? FromJozz { get; set; }
    public int? ToJozz { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedByTeacherId { get; set; }
    public bool CanEdit { get; set; }
}
