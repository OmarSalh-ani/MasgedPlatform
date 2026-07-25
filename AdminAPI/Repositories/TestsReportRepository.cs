using AdminAPI.Data;
using AdminAPI.DTOs.Tests;
using AdminAPI.Repositories.Interfaces;
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

    public Task<List<TestsReportSourceRow>> GetReportRowsAsync(
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

        return query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TestsReportSourceRow
            {
                StudentId = t.StudentId,
                StudentName = t.Student != null ? t.Student.StudentName : string.Empty,
                ParentPhone = t.Student != null ? (t.Student.FatherPhone ?? string.Empty) : string.Empty,
                TeacherName = t.Teacher != null ? t.Teacher.Name : string.Empty,
                CircleName = t.Student != null && t.Student.QuranCircle != null
                    ? t.Student.QuranCircle.Name
                    : string.Empty,
                TestFrom = t.TestFrom ?? string.Empty,
                TestTo = t.TestTo ?? string.Empty,
                TestDate = t.CreatedAt,
                FinalResults = t.FinalResult,
                Notes = t.Notes ?? string.Empty,
                TestName = t.TestName ?? string.Empty,
            })
            .ToListAsync(cancellationToken);
    }
}
