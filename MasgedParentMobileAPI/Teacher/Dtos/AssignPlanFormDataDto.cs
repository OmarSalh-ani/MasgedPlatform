namespace MasgedTeacherMobileAPI.Dtos;

public class AssignPlanFormDataDto
{
    public List<PlanLevelPickDto> PlanLevels { get; set; } = [];
    public List<string> PlanTypes { get; set; } = ["حفظ", "مراجعة"];
    public List<IdNameDto> Surahs { get; set; } = [];
    public List<IdNameDto> JozzList { get; set; } = [];
    public List<IdNameDto> Students { get; set; } = [];
}
