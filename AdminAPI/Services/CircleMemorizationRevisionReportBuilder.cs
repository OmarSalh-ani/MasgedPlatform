using AdminAPI.DTOs.CircleMemorizationRevisionReport;

namespace AdminAPI.Services;

public static class CircleMemorizationRevisionReportBuilder
{
    private static readonly string[] ArabicDayNames =
    [
        "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت"
    ];

    public static readonly string[] ReportStatuses = ["تم الحفظ", "تم المراجعة"];

    public static List<CircleMemorizationRevisionReportRowDto> BuildRows(
        IReadOnlyList<CirclePlanSegmentDto> memorizings,
        IReadOnlyList<CirclePlanSegmentDto> revises)
    {
        var memByKey = memorizings
            .GroupBy(x => (x.StudentId, Date: x.EffectiveDate))
            .ToDictionary(g => g.Key, g => g.ToList());

        var revByKey = revises
            .GroupBy(x => (x.StudentId, Date: x.EffectiveDate))
            .ToDictionary(g => g.Key, g => g.ToList());

        var studentNames = memorizings
            .Concat(revises)
            .GroupBy(x => x.StudentId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.StudentName).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n)) ?? "");

        var keys = memByKey.Keys.Union(revByKey.Keys)
            .OrderBy(k => k.Date)
            .ThenBy(k => studentNames.TryGetValue(k.StudentId, out var n) ? n : "")
            .ThenBy(k => k.StudentId)
            .ToList();

        var rows = new List<CircleMemorizationRevisionReportRowDto>();
        var sequence = 1;

        foreach (var key in keys)
        {
            memByKey.TryGetValue(key, out var memSegments);
            revByKey.TryGetValue(key, out var revSegments);

            var newText = FormatMerged(memSegments);
            var revText = FormatMerged(revSegments);
            if (string.IsNullOrWhiteSpace(newText) && string.IsNullOrWhiteSpace(revText))
                continue;

            studentNames.TryGetValue(key.StudentId, out var studentName);
            rows.Add(new CircleMemorizationRevisionReportRowDto
            {
                Sequence = sequence++,
                StudentName = studentName ?? "",
                DayName = ArabicDayNames[(int)key.Date.DayOfWeek],
                Date = key.Date,
                NewMemorization = newText,
                Revision = revText,
            });
        }

        return rows;
    }

    private static string FormatMerged(IReadOnlyList<CirclePlanSegmentDto>? segments)
    {
        if (segments is null || segments.Count == 0)
            return string.Empty;

        var parts = new List<string>();
        foreach (var group in segments.GroupBy(s => s.SurahId).OrderBy(g => g.Min(x => x.FromAyah)).ThenBy(g => g.Key))
        {
            var surahLabel = EnsureSurahPrefix(group.First().SurahName);
            var merged = MergeRanges(group.Select(x => (x.FromAyah, x.ToAyah)));
            if (merged.Count == 0)
                continue;

            var rangeText = string.Join(" و ", merged.Select(r => $"من {r.From} الى {r.To}"));
            parts.Add($"{surahLabel} {rangeText}");
        }

        return string.Join(" و ", parts);
    }

    public static List<(int From, int To)> MergeRanges(IEnumerable<(int From, int To)> ranges)
    {
        var ordered = ranges
            .Select(r => (From: Math.Min(r.From, r.To), To: Math.Max(r.From, r.To)))
            .OrderBy(r => r.From)
            .ThenBy(r => r.To)
            .ToList();

        if (ordered.Count == 0)
            return [];

        var result = new List<(int From, int To)> { ordered[0] };
        for (var i = 1; i < ordered.Count; i++)
        {
            var current = result[^1];
            var next = ordered[i];
            if (next.From <= current.To + 1)
                result[^1] = (current.From, Math.Max(current.To, next.To));
            else
                result.Add(next);
        }

        return result;
    }

    private static string EnsureSurahPrefix(string name)
    {
        var trimmed = (name ?? "").Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed == "—")
            return "سورة —";
        return trimmed.StartsWith("سورة", StringComparison.Ordinal) ? trimmed : "سورة " + trimmed;
    }
}
