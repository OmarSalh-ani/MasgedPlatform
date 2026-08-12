using AdminAPI.Data;
using AdminAPI.DTOs.QuranCircles;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class QuranCircleRepository(AdminDbContext db) : IQuranCircleRepository
{
    public Task<List<QuranCircleListItemDto>> GetListAsync(
        bool girlOnly,
        bool isAdmin,
        int teacherId,
        int? filterTeacherId,
        CancellationToken cancellationToken = default) =>
        BuildListQuery(girlOnly, isAdmin, teacherId, filterTeacherId)
            .ToListAsync(cancellationToken);

    public Task<List<QuranCircleListItemDto>> GetExportListAsync(
        bool girlOnly,
        CancellationToken cancellationToken = default) =>
        BuildListQuery(girlOnly, isAdmin: true, teacherId: 0, filterTeacherId: null)
            .ToListAsync(cancellationToken);

    private IQueryable<QuranCircleListItemDto> BuildListQuery(
        bool girlOnly,
        bool isAdmin,
        int teacherId,
        int? filterTeacherId)
    {
        var query = db.QuranCircles
            .AsNoTracking()
            .Where(x => x.Teacher != null && x.Teacher.IsGirlTeacher == girlOnly);

        if (filterTeacherId.HasValue)
            query = query.Where(x => x.TeacherId == filterTeacherId.Value);

        if (!isAdmin)
            query = query.Where(x => x.TeacherId == teacherId);

        return query
            .OrderByDescending(x => x.Id)
            .Select(x => new QuranCircleListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                StudentsCount = db.RegisterForms.Count(r => r.QuranCircleId == x.Id),
                TeacherName = x.Teacher!.Name ?? string.Empty,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.Teacher!.Name ?? string.Empty,
                TeacherId = x.TeacherId,
                ForGirls = x.ForGirls ?? false,
            });
    }

    public Task<QuranCircle?> GetByIdWithDaysAsync(int id, CancellationToken cancellationToken = default) =>
        db.QuranCircles
            .Include(c => c.CircleDays)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<QuranCircle> AddAsync(QuranCircle entity, CancellationToken cancellationToken = default)
    {
        await db.QuranCircles.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task ReplaceDaysAsync(
        int circleId,
        IEnumerable<int> dayNumbers,
        CancellationToken cancellationToken = default)
    {
        var existing = await db.CircleDays
            .Where(d => d.CircleId == circleId)
            .ToListAsync(cancellationToken);
        db.CircleDays.RemoveRange(existing);

        foreach (var dayNumber in dayNumbers.Distinct())
        {
            db.CircleDays.Add(new CircleDay { CircleId = circleId, DayNumber = dayNumber });
        }
    }

    public async Task<bool> DeleteWithRelatedAsync(
        int id,
        int? teacherId,
        CancellationToken cancellationToken = default)
    {
        var circle = await db.QuranCircles.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (circle is null)
            return false;

        var students = await db.RegisterForms
            .Where(c => c.QuranCircleId == id)
            .ToListAsync(cancellationToken);
        foreach (var student in students)
            student.QuranCircleId = null;

        var attendances = await db.CircleAttendances
            .Where(c => c.CircleId == id)
            .ToListAsync(cancellationToken);
        foreach (var attendance in attendances)
            attendance.CircleId = null;

        var memorizingCards = await db.StudentMemorizingCards
            .Where(c => c.CircleId == id)
            .ToListAsync(cancellationToken);
        db.StudentMemorizingCards.RemoveRange(memorizingCards);

        if (teacherId.HasValue && students.Count > 0)
        {
            var studentIds = students.Select(s => s.Id).ToList();
            var testHeads = await db.TestHeads
                .Where(t => studentIds.Contains(t.StudentId) && t.TeacherId == teacherId.Value)
                .ToListAsync(cancellationToken);
            db.TestHeads.RemoveRange(testHeads);
        }

        circle.TeacherId = null;
        db.QuranCircles.Remove(circle);
        return true;
    }

    public async Task DeletePlansAndArchiveForCirclesAsync(
        IReadOnlyList<int> circleIds,
        CancellationToken cancellationToken = default)
    {
        if (circleIds.Count == 0)
            return;

        var studentIds = await db.RegisterForms
            .Where(r => r.QuranCircleId != null && circleIds.Contains(r.QuranCircleId.Value))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        if (studentIds.Count > 0)
        {
            var planIds = await db.StudentPlans
                .Where(p => studentIds.Contains(p.StudentId))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (planIds.Count > 0)
            {
                var logs = await db.StudentPlanItemLogs
                    .Where(l => planIds.Contains(l.PlanId))
                    .ToListAsync(cancellationToken);
                db.StudentPlanItemLogs.RemoveRange(logs);

                var memorizings = await db.StudentPlanMemorizings
                    .Where(m => planIds.Contains(m.PlanId))
                    .ToListAsync(cancellationToken);
                db.StudentPlanMemorizings.RemoveRange(memorizings);

                var revises = await db.StudentPlanRevises
                    .Where(r => planIds.Contains(r.PlanId))
                    .ToListAsync(cancellationToken);
                db.StudentPlanRevises.RemoveRange(revises);

                var plans = await db.StudentPlans
                    .Where(p => planIds.Contains(p.Id))
                    .ToListAsync(cancellationToken);
                db.StudentPlans.RemoveRange(plans);
            }
        }

        var memorizingCards = await db.StudentMemorizingCards
            .Where(c => circleIds.Contains(c.CircleId))
            .ToListAsync(cancellationToken);
        db.StudentMemorizingCards.RemoveRange(memorizingCards);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
