using MasgedTeacherMobileAPI.Entities;
using System.Globalization;

namespace MasgedTeacherMobileAPI.Helpers;

internal static class CircleMemorizationRevisionReportArchiveMapper
{
    private const string TypeMemorization = "حفظ";
    private const string TypeRevision = "مراجعة";

    internal sealed record ArchiveSegment(
        int StudentId,
        string StudentName,
        DateTime Date,
        int SurahId,
        string SurahName,
        int FromAyah,
        int ToAyah,
        bool IsMemorization);

    public static ArchiveSegment? Map(StudentMemorizingCard card)
    {
        var isMem = IsMemorization(card) && IsMemorizationCompleted(card);
        var isRev = IsRevision(card) && IsRevisionCompleted(card);
        if (!isMem && !isRev)
            return null;

        if (!TryParseAyahRange(card.TestFrom, card.TestTo, out var fromAyah, out var toAyah))
            return null;

        var effectiveDate = card.CreatedAt.Date;
        if (effectiveDate == default)
            return null;

        return new ArchiveSegment(
            card.StudentId,
            card.RegisterForm?.StudentName ?? string.Empty,
            effectiveDate,
            SurahKey(card.SurahName, card.Id),
            card.SurahName?.Trim() ?? "—",
            fromAyah,
            toAyah,
            isMem);
    }

    private static bool IsMemorization(StudentMemorizingCard card) =>
        string.Equals(card.TheType.Trim(), TypeMemorization, StringComparison.Ordinal);

    private static bool IsRevision(StudentMemorizingCard card) =>
        string.Equals(card.TheType.Trim(), TypeRevision, StringComparison.Ordinal);

    private static bool IsMemorizationCompleted(StudentMemorizingCard card) =>
        string.Equals(card.IsSaveDone?.Trim(), "نعم", StringComparison.Ordinal);

    private static bool IsRevisionCompleted(StudentMemorizingCard card) =>
        string.Equals(card.IsDone?.Trim(), "نعم", StringComparison.Ordinal);

    private static bool TryParseAyahRange(string fromText, string toText, out int fromAyah, out int toAyah)
    {
        fromAyah = 0;
        toAyah = 0;
        if (!int.TryParse(fromText.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out fromAyah))
            return false;
        if (!int.TryParse(toText.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out toAyah))
            return false;
        return fromAyah > 0 && toAyah > 0;
    }

    private static int SurahKey(string? surahName, int fallbackId)
    {
        var trimmed = surahName?.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return -Math.Abs(fallbackId);

        return -Math.Abs(trimmed.GetHashCode(StringComparison.Ordinal));
    }
}
