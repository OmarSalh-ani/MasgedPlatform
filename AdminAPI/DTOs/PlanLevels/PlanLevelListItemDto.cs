namespace AdminAPI.DTOs.PlanLevels;

public class PlanLevelListItemDto
{
    public int Id { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public byte UnitType { get; set; }
    public string UnitTypeDisplay { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsGlobal { get; set; }
}
