using MasgedTeacherMobileAPI.Dtos;

namespace MasgedTeacherMobileAPI.Helpers;

public static class StudentTestsHelper
{
    public static string CalculateGrade(decimal? totalScore) =>
        totalScore.HasValue
            ? TestCertificateHelper.CalculateGrade(totalScore.Value)
            : "ضعيف";

    public static List<StudentTestRowDto> GetValidRows(IEnumerable<StudentTestRowDto>? rows) =>
        (rows ?? [])
            .Where(row => !string.IsNullOrEmpty(row.Degree) &&
                          (!string.IsNullOrEmpty(row.FromSurah) ||
                           !string.IsNullOrEmpty(row.ToSurah) ||
                           !string.IsNullOrEmpty(row.Question)))
            .ToList();

    public static decimal ComputeFinalResultFromRows(IReadOnlyList<StudentTestRowDto> validRows)
    {
        if (validRows.Count == 0)
            return 0;

        decimal total = 0;
        foreach (var row in validRows)
        {
            if (decimal.TryParse(row.Degree, out var degree))
                total += degree;
        }

        return total / validRows.Count;
    }
}
