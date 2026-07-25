using AdminAPI.Models.Enums;

namespace AdminAPI.Helpers;

public static class PlanLevelUnitDisplay
{
    public static string GetUnitDisplay(byte unitType) =>
        ((PlanUnitType)unitType) switch
        {
            PlanUnitType.Page => "صفحة",
            PlanUnitType.QuarterPage => "ربع",
            PlanUnitType.Jozz => "جزء",
            PlanUnitType.Line => "سطر",
            _ => string.Empty
        };
}
