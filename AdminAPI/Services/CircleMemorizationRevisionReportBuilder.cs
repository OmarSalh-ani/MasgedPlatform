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

            var newChips = BuildChips(memSegments);
            var revChips = BuildChips(revSegments);
            var newText = FormatMerged(newChips);
            var revText = FormatMerged(revChips);
            if (newChips.Count == 0 && revChips.Count == 0)
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
                NewMemorizationChips = newChips,
                RevisionChips = revChips,
            });
        }

        return rows;
    }

    private static string FormatMerged(IReadOnlyList<CircleMemorizationSurahChipDto> chips)
    {
        if (chips.Count == 0)
            return string.Empty;

        return string.Join(" و ", chips.Select(chip =>
            string.IsNullOrWhiteSpace(chip.RangeText)
                ? chip.Title
                : $"{chip.Title} {chip.RangeText}"));
    }

    private static List<CircleMemorizationSurahChipDto> BuildChips(IReadOnlyList<CirclePlanSegmentDto>? segments)
    {
        if (segments is null || segments.Count == 0)
            return [];

        segments = DedupeByAyahRange(segments);

        var chips = new List<CircleMemorizationSurahChipDto>();
        foreach (var group in segments
                     .GroupBy(s => s.SurahId)
                     .OrderBy(g => g.Min(x => x.FromAyah))
                     .ThenBy(g => g.Key))
        {
            var first = group.First();
            if (IsJuzHizbUnit(first.SurahName))
            {
                chips.Add(new CircleMemorizationSurahChipDto
                {
                    Title = FormatJuzHizbLabel(first.SurahName, first.FromAyah),
                });
                continue;
            }

            var merged = MergeRanges(group.Select(x => (x.FromAyah, x.ToAyah)));
            if (merged.Count == 0)
                continue;

            var rangeText = string.Join(" و ", merged.Select(r => $"من {r.From} الى {r.To}"));
            chips.Add(new CircleMemorizationSurahChipDto
            {
                Title = EnsureSurahPrefix(first.SurahName),
                RangeText = rangeText,
            });
        }

        return chips;
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

    private static List<CirclePlanSegmentDto> DedupeByAyahRange(IReadOnlyList<CirclePlanSegmentDto> segments) =>
        segments
            .GroupBy(s => (
                From: Math.Min(s.FromAyah, s.ToAyah),
                To: Math.Max(s.FromAyah, s.ToAyah)))
            .Select(g => g.OrderByDescending(s => SegmentNamePriority(s.SurahName)).First())
            .ToList();

    private static int SegmentNamePriority(string? name)
    {
        var trimmed = (name ?? "").Trim();
        if (IsJuzHizbUnit(trimmed))
            return 2;
        if (!string.IsNullOrEmpty(trimmed) && trimmed != "—")
            return 1;
        return 0;
    }

    private static string EnsureSurahPrefix(string name)
    {
        var trimmed = (name ?? "").Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed == "—")
            return "سورة —";
        if (IsJuzHizbUnit(trimmed))
            return trimmed;
        return trimmed.StartsWith("سورة", StringComparison.Ordinal) ? trimmed : "سورة " + trimmed;
    }

    private static bool IsJuzHizbUnit(string? name)
    {
        var trimmed = (name ?? "").Trim();
        return trimmed is "جزء" or "حزب";
    }

    private static string FormatJuzHizbLabel(string unitType, int number) =>
        $"{unitType.Trim()} {number}";
}
