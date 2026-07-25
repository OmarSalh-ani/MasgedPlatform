using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class SavePlanLevelRequestDto
{
    [Required]
    public string LevelName { get; set; } = string.Empty;

    public byte UnitType { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
