using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;

namespace MasgedTeacherMobileAPI.Helpers;

public static class CircleMemorizationRevisionReportBuilder
{
    private sealed record Segment(int SurahId, string SurahName, int FromAyah, int ToAyah);

    /// <summary>
    /// The day the work was actually assessed. Rows recorded before completion dates were
    /// stamped, or planned but never assessed, fall back to their planned day.
    /// </summary>
    public static DateTime EffectiveDate(StudentPlanMemorizing row) =>
        (row.MemorizeDate ?? row.PlanDate).Date;

    /// <inheritdoc cref="EffectiveDate(StudentPlanMemorizing)"/>
    public static DateTime EffectiveDate(StudentPlanRevise row) =>
        (row.ReviseDate ?? row.PlanDate).Date;

    public static List<CircleMemorizationRevisionReportRowDto> BuildRows(
        IReadOnlyList<StudentPlanMemorizing> memorizings,
        IReadOnlyList<StudentPlanRevise> revises,
        IReadOnlyList<StudentMemorizingCard>? archiveCards = null)
    {
        var memByKey = memorizings
            .GroupBy(x => (x.StudentId, Date: EffectiveDate(x)))
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new Segment(
                    x.SurahId,
                    x.QuranSurah?.NameAr ?? "—",
                    x.FromAyahNumber,
                    x.ToAyahNumber)).ToList());

        var revByKey = revises
            .GroupBy(x => (x.StudentId, Date: EffectiveDate(x)))
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new Segment(
                    x.SurahId,
                    x.QuranSurah?.NameAr ?? "—",
                    x.FromAyahNumber,
                    x.ToAyahNumber)).ToList());

        if (archiveCards is { Count: > 0 })
        {
            foreach (var card in archiveCards)
            {
                var mapped = CircleMemorizationRevisionReportArchiveMapper.Map(card);
                if (mapped is null)
                    continue;

                var key = (mapped.StudentId, mapped.Date);
                var segment = new Segment(mapped.SurahId, mapped.SurahName, mapped.FromAyah, mapped.ToAyah);
                if (mapped.IsMemorization)
                {
                    if (!memByKey.TryGetValue(key, out var list))
                    {
                        list = [];
                        memByKey[key] = list;
                    }

                    list.Add(segment);
                }
                else
                {
                    if (!revByKey.TryGetValue(key, out var list))
                    {
                        list = [];
                        revByKey[key] = list;
                    }

                    list.Add(segment);
                }
            }
        }

        var studentNames = memorizings
            .Select(x => (x.StudentId, Name: x.RegisterForm?.StudentName ?? ""))
            .Concat(revises.Select(x => (x.StudentId, Name: x.RegisterForm?.StudentName ?? "")))
            .Concat((archiveCards ?? [])
                .Select(x => (x.StudentId, Name: x.RegisterForm?.StudentName ?? "")))
            .GroupBy(x => x.StudentId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Name).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n)) ?? "");

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
                DayName = AttendanceHelper.GetArabicDayName(key.Date.DayOfWeek),
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

    private static List<CircleMemorizationSurahChipDto> BuildChips(IReadOnlyList<Segment>? segments)
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

    /// <summary>Public for unit-style verification of merge rules.</summary>
    public static List<(int From, int To)> MergeRanges(IEnumerable<(int From, int To)> ranges)
    {
        var ordered = ranges
            .Select(r => (
                From: Math.Min(r.From, r.To),
                To: Math.Max(r.From, r.To)))
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
            // Overlapping or adjacent (1-3 and 3-6 / 1-3 and 4-6)
            if (next.From <= current.To + 1)
            {
                result[^1] = (current.From, Math.Max(current.To, next.To));
            }
            else
            {
                result.Add(next);
            }
        }

        return result;
    }

    private static List<Segment> DedupeByAyahRange(IReadOnlyList<Segment> segments) =>
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
        if (trimmed.StartsWith("سورة", StringComparison.Ordinal))
            return trimmed;
        return "سورة " + trimmed;
    }

    private static bool IsJuzHizbUnit(string? name)
    {
        var trimmed = (name ?? "").Trim();
        return trimmed is "جزء" or "حزب";
    }

    private static string FormatJuzHizbLabel(string unitType, int number) =>
        $"{unitType.Trim()} {number}";
}
