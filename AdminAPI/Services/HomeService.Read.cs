using AdminAPI.DTOs.Home;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public partial class HomeService
{
    public async Task<byte[]> ExportExcelAsync(
        HomeListFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var teacherCircleIds = await GetTeacherCircleIdsAsync(cancellationToken);
        var query = HomeStudentQueryBuilder.Build(db, currentUser, filters, teacherCircleIds);

        var students = await query.Select(x => new HomeExportRow
        {
            Id = x.Id,
            StudentName = x.FullName != null && x.FullName != string.Empty ? x.FullName : x.StudentName,
            FatherName = x.FatherName,
            Birthdate = x.Birthdate,
            Age = x.Age,
            StudentGender = x.StudentGender,
            FatherPhone = x.FatherPhone,
            FatherPhone2 = x.FatherPhone2,
            StudentPhone = x.StudentPhone,
            CreatedAt = x.CreatedAt,
            CircleName = x.QuranCircle != null ? x.QuranCircle.Name : string.Empty,
            IsSpecial = x.IsSpecial ? "نعم" : "لا",
            WomanActivityType = x.WomanActivity != null ? x.WomanActivity.Name : string.Empty,
            LearnCertificate = x.LearnCertificate,
            ThePassword = x.ThePassword,
            LeaveCount = x.CircleAttendances.Count(c => c.StudentId == x.Id && !c.IsHere),
            CompleteFollowup = x.ParentFollowup != null ? "نعم" : "لا",
            IsElite = x.IsElite,
        }).ToListAsync(cancellationToken);

        return HomeExcelExporter.Build(students);
    }

    public async Task<List<HomeStudentTestDto>> GetStudentTestsAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        return await db.TestHeads
            .AsNoTracking()
            .Where(t => t.StudentId == studentId)
            .OrderByDescending(t => t.TestDate)
            .Select(t => new HomeStudentTestDto
            {
                TestName = t.TestDate.ToString("yyyy/MM/dd HH:mm"),
                TestType = t.TestType ?? string.Empty,
                From = t.TestFrom ?? string.Empty,
                To = t.TestTo ?? string.Empty,
                TestDegree = t.FinalResult.ToString("N0"),
                Notes = t.Notes ?? string.Empty,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<HomeStudentReviewDto>> GetStudentReviewsAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        return await db.StudentMemorizingCards
            .AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new HomeStudentReviewDto
            {
                ReviewType = r.TheType ?? string.Empty,
                CreatedAt = r.CreatedAt != null ? r.CreatedAt.Value.ToString("yyyy-MM-dd dddd") : string.Empty,
                TestFrom = r.TestFrom ?? string.Empty,
                TestTo = r.TestTo ?? string.Empty,
                SurahName = r.SurahName ?? string.Empty,
                IsDone = r.IsDone ?? string.Empty,
                Notes = r.Notes ?? string.Empty,
                ParentNotes = r.ParentNotes ?? string.Empty,
                IsSaveDone = r.IsSaveDone ?? string.Empty,
                DisplayNotes = r.ParentNotes ?? string.Empty,
            })
            .ToListAsync(cancellationToken);
    }
}
