using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;

namespace MasgedTeacherMobileAPI.Helpers;

public static class QuranPageBuilder
{
    private const int DefaultLinesPerPage = 15;
    private const string BismillahText = "بِسْمِ ٱللَّهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ";

    public static List<QuranLineDto> BuildPageLines(List<HolyQuran> rows)
    {
        var maxLine = rows.Count > 0 ? rows.Max(x => x.line_end ?? 0) : DefaultLinesPerPage;
        if (maxLine < DefaultLinesPerPage)
            maxLine = DefaultLinesPerPage;

        var lineParts = new Dictionary<int, List<string>>();
        for (var i = 1; i <= maxLine; i++)
            lineParts[i] = [];

        foreach (var ayah in rows)
        {
            var start = ayah.line_start is null or <= 0 ? 1 : ayah.line_start.Value;
            var end = ayah.line_end < start ? start : (ayah.line_end ?? start);
            var span = end - start + 1;

            var rawText = (ayah.aya_text_emlaey ?? "").Trim();
            if (string.IsNullOrWhiteSpace(rawText))
                continue;

            var ayahMarker = " ﴿" + ayah.aya_no + "﴾";

            if (span == 1)
            {
                lineParts[start].Add(rawText + ayahMarker);
                continue;
            }

            var segments = SplitAyahIntoLineSegments(rawText, span);
            for (var i = 0; i < span; i++)
            {
                var lineNo = start + i;
                if (!lineParts.ContainsKey(lineNo))
                    lineParts[lineNo] = [];

                var seg = i < segments.Count ? segments[i] : "";
                if (i == span - 1)
                    seg = (seg ?? "").TrimEnd() + ayahMarker;
                if (!string.IsNullOrWhiteSpace(seg))
                    lineParts[lineNo].Add(seg.Trim());
            }
        }

        var result = new List<QuranLineDto>();

        if (rows.Count > 0)
        {
            var firstSurah = rows[0].sura_name_ar;
            if (!string.IsNullOrWhiteSpace(firstSurah))
            {
                result.Add(new QuranLineDto
                {
                    LineNumber = 0,
                    Text = firstSurah,
                    CssClass = "surah-header"
                });
            }
        }

        for (var line = 1; line <= maxLine; line++)
        {
            var text = lineParts.TryGetValue(line, out var parts) ? string.Join(" ", parts) : "";
            if (string.IsNullOrWhiteSpace(text))
                continue;

            var css = "quran-line";
            if (text.Trim() == BismillahText)
                css += " bismillah-line";

            result.Add(new QuranLineDto
            {
                LineNumber = line,
                Text = text,
                CssClass = css
            });
        }

        return result;
    }

    public static List<HolyQuran> ApplySurahFilter(
        List<HolyQuran> rows,
        int targetSurahId,
        int fromAyah,
        int toAyah,
        out bool isFiltered,
        out string surahName)
    {
        isFiltered = false;
        surahName = rows.Count > 0 ? rows[0].sura_name_ar ?? "" : "";

        if (targetSurahId <= 0)
            return rows;

        var filteredRows = rows.Where(r => r.sura_no == targetSurahId).ToList();
        if (fromAyah > 0)
            filteredRows = filteredRows.Where(r => r.aya_no >= fromAyah).ToList();
        if (toAyah > 0)
            filteredRows = filteredRows.Where(r => r.aya_no <= toAyah).ToList();

        if (filteredRows.Count == 0)
            return rows;

        isFiltered = true;
        surahName = "[" + (filteredRows[0].sura_name_ar ?? surahName) + "]";
        return filteredRows;
    }

    public static List<int> GetHighlightAyahNumbers(
        List<HolyQuran> rows,
        int targetSurahId,
        int fromAyah,
        int toAyah)
    {
        if (targetSurahId <= 0 || fromAyah <= 0)
            return [];

        return rows
            .Where(r => r.sura_no == targetSurahId
                        && r.aya_no >= fromAyah
                        && (toAyah == 0 || r.aya_no <= toAyah))
            .Select(r => r.aya_no)
            .Distinct()
            .OrderBy(n => n)
            .ToList();
    }

    public static string BuildPageMeta(int pageJozz, string surahName) =>
        (pageJozz > 0 ? "الجزء " + pageJozz + " - " : "") +
        (!string.IsNullOrWhiteSpace(surahName) ? surahName : "المصحف الشريف");

    private static List<string> SplitAyahIntoLineSegments(string ayahText, int span)
    {
        var normalized = (ayahText ?? "").Replace("\r\n", "\n").Replace('\r', '\n');

        if (normalized.Contains('\n'))
        {
            var parts = normalized.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToList();
            if (parts.Count > 0)
                return FitSegments(parts, span);
        }

        if (normalized.Contains('|'))
        {
            var parts = normalized.Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToList();
            if (parts.Count > 0)
                return FitSegments(parts, span);
        }

        var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        if (words.Count == 0)
            return FitSegments([normalized], span);

        var segments = new List<string>(span);
        var total = words.Count;
        var idx = 0;
        for (var i = 0; i < span; i++)
        {
            var remainingSegments = span - i;
            var remainingWords = total - idx;
            var take = Math.Max(1, (int)Math.Ceiling(remainingWords / (double)remainingSegments));
            var chunk = words.Skip(idx).Take(take).ToList();
            idx += chunk.Count;
            segments.Add(string.Join(" ", chunk));
        }

        return FitSegments(segments, span);
    }

    private static List<string> FitSegments(List<string> parts, int span)
    {
        parts = parts.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
        if (parts.Count == 0)
            parts.Add("");

        if (parts.Count == span)
            return parts;

        if (parts.Count > span)
        {
            var head = parts.Take(span - 1).ToList();
            head.Add(string.Join(" ", parts.Skip(span - 1)));
            return head;
        }

        while (parts.Count < span)
            parts.Add("");
        return parts;
    }
}
