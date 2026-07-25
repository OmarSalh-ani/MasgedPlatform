namespace MasgedTeacherMobileAPI.Dtos;

public class PlanLevelFormDataDto
{
    public List<UnitTypeOptionDto> UnitTypes { get; set; } = [];
    public List<IdNameDto> Surahs { get; set; } = [];
    public List<IdNameDto> JozzList { get; set; } = [];
    public string DefaultFromDate { get; set; } = string.Empty;
    public string DefaultToDate { get; set; } = string.Empty;
}

public class UnitTypeOptionDto
{
    public byte Value { get; set; }
    public string Label { get; set; } = string.Empty;
}
