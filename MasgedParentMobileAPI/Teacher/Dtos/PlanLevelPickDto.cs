namespace MasgedTeacherMobileAPI.Dtos;

public class PlanLevelPickDto
{
    public int Id { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public byte UnitType { get; set; }
    public bool UsesJozzInput { get; set; }
}
