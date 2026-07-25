using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class SaveReadyPlanRequestDto
{
    public int? PlanLevelId { get; set; }

    public string? LevelName { get; set; }

    public byte? UnitType { get; set; }

    public int? Quantity { get; set; }

    public int FromSurahId { get; set; }

    public int ToSurahId { get; set; }

    public int? FromAyah { get; set; }

    public int? ToAyah { get; set; }

    public int? FromJozz { get; set; }

    public int? ToJozz { get; set; }

    [Required]
    public string FromDate { get; set; } = string.Empty;

    [Required]
    public string ToDate { get; set; } = string.Empty;
}
