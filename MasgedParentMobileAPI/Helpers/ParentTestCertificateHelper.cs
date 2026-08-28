using System.Globalization;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Helpers;

namespace MasgedParentMobileAPI.Helpers;

public static class ParentTestCertificateHelper
{
    private static readonly string[] ValidTestPeriods =
    [
        "الفصل الأول",
        "الفصل الثاني",
        "الفصل الثالث"
    ];

    public static string NormalizeTestPeriod(string? testPeriod)
    {
        var period = string.IsNullOrWhiteSpace(testPeriod) ? "الفصل الأول" : testPeriod.Trim();
        return ValidTestPeriods.Contains(period) ? period : "الفصل الأول";
    }

    public static ParentTestCertificateListItemDto BuildListItem(
        TestHead testHead,
        RegisterForm student)
    {
        var memorization = testHead.MemorizationScore ?? 60m;
        var tajweed = testHead.TajweedScore ?? 30m;
        var revision = testHead.RevisionScore ?? 10m;
        var total = memorization + tajweed + revision;

        return new ParentTestCertificateListItemDto
        {
            TestId = testHead.Id,
            StudentId = testHead.StudentId,
            StudentName = student.StudentName ?? string.Empty,
            TestDate = testHead.TestDate.ToString("yyyy/MM/dd"),
            Grade = TestCertificateHelper.CalculateGrade(total),
            TotalScore = total.ToString("N0", CultureInfo.InvariantCulture),
            TestFrom = testHead.TestFrom ?? string.Empty,
            TestTo = testHead.TestTo ?? string.Empty,
        };
    }

    public static TestCertificateDto BuildCertificateDto(
        TestHead testHead,
        RegisterForm student,
        QuranCircle circle,
        string testPeriod)
    {
        var teacherHead = new MasgedTeacherMobileAPI.Entities.TestHead
        {
            Id = testHead.Id,
            StudentId = testHead.StudentId,
            CircleId = testHead.CircleId,
            TeacherId = testHead.TeacherId,
            TestFrom = testHead.TestFrom,
            TestTo = testHead.TestTo,
            SurahName = testHead.SurahName,
            HezbNumber = testHead.HezbNumber,
            TestDate = testHead.TestDate,
            FinalResult = testHead.FinalResult,
            MemorizationScore = testHead.MemorizationScore,
            TajweedScore = testHead.TajweedScore,
            RevisionScore = testHead.RevisionScore,
            TotalScore = testHead.TotalScore,
            Grade = testHead.Grade,
            Notes = testHead.Notes,
            CreatedAt = testHead.CreatedAt,
            TestName = testHead.TestName,
            TestType = testHead.TestType,
        };

        var teacherStudent = new MasgedTeacherMobileAPI.Entities.RegisterForm
        {
            Id = student.Id,
            StudentName = student.StudentName,
        };

        var teacherCircle = new MasgedTeacherMobileAPI.Entities.QuranCircle
        {
            Id = circle.Id,
            Name = circle.Name,
        };

        return TestCertificateHelper.BuildDto(teacherHead, teacherStudent, teacherCircle, testPeriod);
    }
}
