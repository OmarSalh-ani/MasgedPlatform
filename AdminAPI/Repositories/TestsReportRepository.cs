using AdminAPI.Data;
using AdminAPI.DTOs.Tests;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class TestsReportRepository(AdminDbContext db) : ITestsReportRepository
{
    public Task<List<TestsReportCircleOptionDto>> GetCirclesAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default) =>
        db.QuranCircles
            .AsNoTracking()
            .Where(c => c.ForGirls == isGirlTeacher)
            .OrderBy(c => c.Name)
            .Select(c => new TestsReportCircleOptionDto
            {
                Id = c.Id,
                Name = c.Name,
            })
            .ToListAsync(cancellationToken);

    public async Task<List<TestsReportSourceRow>> GetReportRowsAsync(
        DateTime fromDate,
        DateTime toDateExclusive,
        int? circleId,
        CancellationToken cancellationToken = default)
    {
        var query = db.TestHeads
            .AsNoTracking()
            .Where(t => t.CreatedAt >= fromDate && t.CreatedAt < toDateExclusive);

        if (circleId is > 0)
            query = query.Where(t => t.Student != null && t.Student.QuranCircleId == circleId.Value);

        var rows = await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.StudentId,
                StudentName = t.Student != null ? t.Student.StudentName : string.Empty,
                ParentPhone = t.Student != null ? (t.Student.FatherPhone ?? string.Empty) : string.Empty,
                TeacherName = t.Teacher != null ? t.Teacher.Name : string.Empty,
                CircleName = t.Student != null && t.Student.QuranCircle != null
                    ? t.Student.QuranCircle.Name
                    : string.Empty,
                t.TestFrom,
                t.TestTo,
                t.SurahName,
                t.HezbNumber,
                t.CreatedAt,
                t.FinalResult,
                t.Notes,
                t.TestName,
                t.TestType,
            })
            .ToListAsync(cancellationToken);

        return rows
            .Select(t => new TestsReportSourceRow
            {
                StudentId = t.StudentId,
                StudentName = t.StudentName ?? string.Empty,
                ParentPhone = t.ParentPhone ?? string.Empty,
                TeacherName = t.TeacherName ?? string.Empty,
                CircleName = t.CircleName ?? string.Empty,
                TestFrom = TestRangeResolver.ResolveFrom(t.TestFrom, t.HezbNumber, t.SurahName),
                TestTo = TestRangeResolver.ResolveTo(t.TestTo, t.HezbNumber, t.SurahName),
                TestDate = t.CreatedAt,
                FinalResults = t.FinalResult,
                Notes = t.Notes ?? string.Empty,
                TestName = TestRangeResolver.ResolveTestType(t.TestName, t.TestType),
            })
            .ToList();
    }
}
