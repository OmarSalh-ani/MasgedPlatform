namespace MasgedTeacherMobileAPI.Helpers;

public static class ManualPlanRowHelper
{
    public const string Prefix = "__manual__:";
    public const int PlaceholderSurahId = 1;
    public const string DefaultLevel = "—";

    public static bool IsManual(string? memorizationLevel) =>
        (memorizationLevel ?? string.Empty).StartsWith(Prefix, StringComparison.Ordinal);

    public static string Format(string surahName) => Prefix + surahName.Trim();

    public static string ExtractName(string memorizationLevel)
    {
        if (!IsManual(memorizationLevel))
            return memorizationLevel;

        return memorizationLevel[Prefix.Length..].Trim();
    }

    public static string ResolveLevel(string? inputSurahName, string? existingLevel = null)
    {
        if (!string.IsNullOrWhiteSpace(inputSurahName))
            return Format(inputSurahName);

        if (IsManual(existingLevel))
            return existingLevel!;

        return DefaultLevel;
    }

    public static bool IsPlanLevelCandidate(string? memorizationLevel) =>
        !string.IsNullOrWhiteSpace(memorizationLevel) && !IsManual(memorizationLevel);
}
