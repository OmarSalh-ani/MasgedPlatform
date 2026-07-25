using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class AssignPlanRequestDto
{
    [Required]
    [MinLength(1)]
    public List<int> StudentIds { get; set; } = [];

    public int PlanLevelId { get; set; }

    public int FromSurahId { get; set; }

    public int ToSurahId { get; set; }

    public int? FromJozz { get; set; }

    public int? ToJozz { get; set; }

    [Required]
    public string FromDate { get; set; } = string.Empty;

    [Required]
    public string ToDate { get; set; } = string.Empty;

    [Required]
    public string PlanType { get; set; } = "حفظ";

    public int? FromAyahNumber { get; set; }

    public int? ToAyahNumber { get; set; }
}
