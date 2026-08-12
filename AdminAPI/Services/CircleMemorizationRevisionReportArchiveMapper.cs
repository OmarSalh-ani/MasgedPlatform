using AdminAPI.DTOs.CircleMemorizationRevisionReport;
using AdminAPI.Models;
using System.Globalization;

namespace AdminAPI.Services;

internal static class CircleMemorizationRevisionReportArchiveMapper
{
    private const string TypeMemorization = "حفظ";
    private const string TypeRevision = "مراجعة";

    public static CirclePlanSegmentDto? ToMemorizingSegment(StudentMemorizingCard card)
    {
        if (!IsMemorization(card) || !IsMemorizationCompleted(card))
            return null;

        if (!TryParseAyahRange(card.TestFrom, card.TestTo, out var fromAyah, out var toAyah))
            return null;

        var effectiveDate = (card.CreatedAt ?? default).Date;
        if (effectiveDate == default)
            return null;

        return new CirclePlanSegmentDto
        {
            StudentId = card.StudentId,
            StudentName = card.Student?.StudentName ?? string.Empty,
            PlanDate = effectiveDate,
            CompletedDate = effectiveDate,
            SurahId = SurahKey(card.SurahName),
            SurahName = card.SurahName?.Trim() ?? "—",
            FromAyah = fromAyah,
            ToAyah = toAyah,
        };
    }

    public static CirclePlanSegmentDto? ToReviseSegment(StudentMemorizingCard card)
    {
        if (!IsRevision(card) || !IsRevisionCompleted(card))
            return null;

        if (!TryParseAyahRange(card.TestFrom, card.TestTo, out var fromAyah, out var toAyah))
            return null;

        var effectiveDate = (card.CreatedAt ?? default).Date;
        if (effectiveDate == default)
            return null;

        return new CirclePlanSegmentDto
        {
            StudentId = card.StudentId,
            StudentName = card.Student?.StudentName ?? string.Empty,
            PlanDate = effectiveDate,
            CompletedDate = effectiveDate,
            SurahId = SurahKey(card.SurahName, card.Id),
            SurahName = card.SurahName?.Trim() ?? "—",
            FromAyah = fromAyah,
            ToAyah = toAyah,
        };
    }

    private static bool IsMemorization(StudentMemorizingCard card) =>
        string.Equals(card.TheType?.Trim(), TypeMemorization, StringComparison.Ordinal);

    private static bool IsRevision(StudentMemorizingCard card) =>
        string.Equals(card.TheType?.Trim(), TypeRevision, StringComparison.Ordinal);

    private static bool IsMemorizationCompleted(StudentMemorizingCard card) =>
        string.Equals(card.IsSaveDone?.Trim(), "نعم", StringComparison.Ordinal);

    private static bool IsRevisionCompleted(StudentMemorizingCard card) =>
        string.Equals(card.IsDone?.Trim(), "نعم", StringComparison.Ordinal);

    private static bool TryParseAyahRange(string? fromText, string? toText, out int fromAyah, out int toAyah)
    {
        fromAyah = 0;
        toAyah = 0;
        if (!int.TryParse(fromText?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out fromAyah))
            return false;
        if (!int.TryParse(toText?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out toAyah))
            return false;
        return fromAyah > 0 && toAyah > 0;
    }

    private static int SurahKey(string? surahName, int fallbackId = 0)
    {
        var trimmed = surahName?.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return -Math.Abs(fallbackId);

        return -Math.Abs(trimmed.GetHashCode(StringComparison.Ordinal));
    }
}
