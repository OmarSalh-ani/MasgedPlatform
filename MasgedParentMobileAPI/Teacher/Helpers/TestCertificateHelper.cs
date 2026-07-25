using System.Globalization;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;

namespace MasgedTeacherMobileAPI.Helpers;

public static class TestCertificateHelper
{
    public static TestCertificateDto BuildDto(
        TestHead testHead,
        RegisterForm student,
        QuranCircle circle,
        string testPeriod = "الفصل الأول")
    {
        var memorization = testHead.MemorizationScore ?? 60m;
        var tajweed = testHead.TajweedScore ?? 30m;
        var revision = testHead.RevisionScore ?? 10m;
        var total = memorization + tajweed + revision;
        var grade = CalculateGrade(total);

        return new TestCertificateDto
        {
            TestId = testHead.Id,
            StudentName = student.StudentName ?? "غير محدد",
            CircleName = circle.Name ?? "غير محدد",
            TestPeriod = testPeriod,
            HizbCells = ParseHizbCells(testHead.HezbNumber),
            MemorizationScore = memorization.ToString("N0", CultureInfo.InvariantCulture),
            TajweedScore = tajweed.ToString("N0", CultureInfo.InvariantCulture),
            RevisionScore = revision.ToString("N0", CultureInfo.InvariantCulture),
            TotalScore = total.ToString("N0", CultureInfo.InvariantCulture) + "%",
            Grade = grade,
            TestDate = testHead.TestDate.ToString("dd/MM/yyyy") + " م"
        };
    }

    public static List<string> ParseHizbCells(string? hezbNumber)
    {
        var cells = new List<string>(8);
        for (var i = 0; i < 8; i++)
            cells.Add("");

        if (string.IsNullOrWhiteSpace(hezbNumber))
            return cells;

        var values = hezbNumber
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(v => v.Trim())
            .ToArray();

        for (var i = 0; i < Math.Min(8, values.Length); i++)
            cells[i] = values[i];

        return cells;
    }

    public static string CalculateGrade(decimal totalScore) =>
        totalScore switch
        {
            >= 90 => "ممتاز",
            >= 80 => "جيد جدا",
            >= 70 => "جيد",
            >= 60 => "متوسط",
            _ => "ضعيف"
        };
}
