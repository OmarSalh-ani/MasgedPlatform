namespace AdminAPI.DTOs.PlanLevels;

public class SavePlanLevelRequestDto
{
    public string LevelName { get; set; } = string.Empty;
    public byte UnitType { get; set; }
    public int Quantity { get; set; }
}
