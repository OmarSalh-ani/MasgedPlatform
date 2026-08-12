using AdminAPI.Data;
using AdminAPI.DTOs.CircleMemorizationRevisionReport;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class CircleMemorizationRevisionReportRepository(AdminDbContext db)
    : ICircleMemorizationRevisionReportRepository
{
    private static readonly string[] ReportStatuses =
        CircleMemorizationRevisionReportBuilder.ReportStatuses;

    public async Task<List<CircleMemorizationTeacherOptionDto>> GetTeachersAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default)
    {
        return await db.Teachers
            .AsNoTracking()
            .Where(t => t.IsGirlTeacher == isGirlTeacher && t.UsersManage == false)
            .OrderBy(t => t.Name)
            .Select(t => new CircleMemorizationTeacherOptionDto { Id = t.Id, Name = t.Name })
            .ToListAsync(cancellationToken);
    }

    public async Task<(string? TeacherName, List<(int Id, string Name)> Circles)?> GetTeacherContextAsync(
        int teacherId,
        bool isGirlTeacher,
        CancellationToken cancellationToken = default)
    {
        var teacher = await db.Teachers
            .AsNoTracking()
            .Where(t => t.Id == teacherId && t.IsGirlTeacher == isGirlTeacher && t.UsersManage == false)
            .Select(t => new { t.Id, t.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher is null)
            return null;

        var circles = await db.QuranCircles
            .AsNoTracking()
            .Where(c => c.TeacherId == teacherId)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync(cancellationToken);

        return (teacher.Name, circles.Select(c => (c.Id, c.Name ?? "")).ToList());
    }

    public Task<List<CirclePlanSegmentDto>> GetMemorizingSegmentsAsync(
        IReadOnlyList<int> circleIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default) =>
        db.StudentPlanMemorizings
            .AsNoTracking()
            .Where(x => x.RegisterForm.QuranCircleId != null
                        && circleIds.Contains(x.RegisterForm.QuranCircleId.Value)
                        && (x.MemorizeDate ?? x.PlanDate) >= from
                        && (x.MemorizeDate ?? x.PlanDate) <= to
                        && x.Status != null
                        && ReportStatuses.Contains(x.Status))
            .Select(x => new CirclePlanSegmentDto
            {
                StudentId = x.StudentId,
                StudentName = x.RegisterForm.StudentName,
                PlanDate = x.PlanDate,
                CompletedDate = x.MemorizeDate,
                SurahId = x.SurahId,
                SurahName = x.QuranSurah.NameAr,
                FromAyah = x.FromAyahNumber,
                ToAyah = x.ToAyahNumber,
            })
            .ToListAsync(cancellationToken);

    public Task<List<CirclePlanSegmentDto>> GetReviseSegmentsAsync(
        IReadOnlyList<int> circleIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default) =>
        db.StudentPlanRevises
            .AsNoTracking()
            .Where(x => x.RegisterForm.QuranCircleId != null
                        && circleIds.Contains(x.RegisterForm.QuranCircleId.Value)
                        && (x.ReviseDate ?? x.PlanDate) >= from
                        && (x.ReviseDate ?? x.PlanDate) <= to
                        && x.Status != null
                        && ReportStatuses.Contains(x.Status))
            .Select(x => new CirclePlanSegmentDto
            {
                StudentId = x.StudentId,
                StudentName = x.RegisterForm.StudentName,
                PlanDate = x.PlanDate,
                CompletedDate = x.ReviseDate,
                SurahId = x.SurahId,
                SurahName = x.QuranSurah.NameAr,
                FromAyah = x.FromAyahNumber,
                ToAyah = x.ToAyahNumber,
            })
            .ToListAsync(cancellationToken);

    public async Task<List<CirclePlanSegmentDto>> GetArchiveMemorizingSegmentsAsync(
        IReadOnlyList<int> circleIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var cards = await db.StudentMemorizingCards
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x => circleIds.Contains(x.CircleId)
                        && x.CreatedAt != null
                        && x.CreatedAt.Value.Date >= from
                        && x.CreatedAt.Value.Date <= to
                        && x.TheType == "حفظ"
                        && x.IsSaveDone == "نعم")
            .ToListAsync(cancellationToken);

        return cards
            .Select(CircleMemorizationRevisionReportArchiveMapper.ToMemorizingSegment)
            .Where(x => x is not null)
            .Cast<CirclePlanSegmentDto>()
            .ToList();
    }

    public async Task<List<CirclePlanSegmentDto>> GetArchiveReviseSegmentsAsync(
        IReadOnlyList<int> circleIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var cards = await db.StudentMemorizingCards
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x => circleIds.Contains(x.CircleId)
                        && x.CreatedAt != null
                        && x.CreatedAt.Value.Date >= from
                        && x.CreatedAt.Value.Date <= to
                        && x.TheType == "مراجعة"
                        && x.IsDone == "نعم")
            .ToListAsync(cancellationToken);

        return cards
            .Select(CircleMemorizationRevisionReportArchiveMapper.ToReviseSegment)
            .Where(x => x is not null)
            .Cast<CirclePlanSegmentDto>()
            .ToList();
    }
}
