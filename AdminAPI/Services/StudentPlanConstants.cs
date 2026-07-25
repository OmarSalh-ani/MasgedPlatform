namespace AdminAPI.Services;

public static class StudentPlanConstants
{
    public static readonly string[] MemorizationLevels = ["محدد الحفظ", "متوسط", "متميز"];
    public static readonly string[] PlanTypes = ["حفظ", "مراجعة", "حفظ ومراجعة"];

    public const string DefaultLevel = "محدد الحفظ";
    public const string TypeMemorizing = "حفظ";
    public const string TypeRevise = "مراجعة";
    public const string TypeBoth = "حفظ ومراجعة";
}

public sealed class ExpandedPlanRow
{
    public int SurahId { get; init; }
    public int FromAyah { get; init; }
    public int ToAyah { get; init; }
    public string PlanType { get; init; } = StudentPlanConstants.TypeMemorizing;
}
