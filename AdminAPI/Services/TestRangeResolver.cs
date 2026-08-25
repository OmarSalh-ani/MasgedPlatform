namespace AdminAPI.Services;

public static class TestRangeResolver
{
    public const string DefaultTestType = "اختبار";

    public static (string From, string To) Resolve(
        string? fromSurah,
        string? toSurah,
        string? hezbNumber,
        string? surahName)
    {
        if (!string.IsNullOrWhiteSpace(fromSurah) || !string.IsNullOrWhiteSpace(toSurah))
            return (TrimOrEmpty(fromSurah), TrimOrEmpty(toSurah));

        var filled = ParseFilledHezbCells(hezbNumber);
        if (filled.Count > 0)
            return (filled[0], filled[^1]);

        return (TrimOrEmpty(surahName), string.Empty);
    }

    public static string ResolveFrom(string? testFrom, string? hezbNumber, string? surahName) =>
        !string.IsNullOrWhiteSpace(testFrom)
            ? testFrom.Trim()
            : Resolve(null, null, hezbNumber, surahName).From;

    public static string ResolveTo(string? testTo, string? hezbNumber, string? surahName) =>
        !string.IsNullOrWhiteSpace(testTo)
            ? testTo.Trim()
            : Resolve(null, null, hezbNumber, surahName).To;

    public static string ResolveTestType(string? testName, string? testType)
    {
        if (!string.IsNullOrWhiteSpace(testName))
            return testName.Trim();
        if (!string.IsNullOrWhiteSpace(testType))
            return testType.Trim();
        return DefaultTestType;
    }

    private static string TrimOrEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private static List<string> ParseFilledHezbCells(string? hezbNumber)
    {
        if (string.IsNullOrWhiteSpace(hezbNumber))
            return [];

        return hezbNumber
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToList();
    }
}
