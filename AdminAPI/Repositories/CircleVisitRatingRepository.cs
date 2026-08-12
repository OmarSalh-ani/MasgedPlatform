using AdminAPI.Data;
using AdminAPI.DTOs.CircleVisitRating;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class CircleVisitRatingRepository(AdminDbContext db) : ICircleVisitRatingRepository
{
    public async Task<(List<CircleVisitRatingListItemDto> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool isAdmin,
        int currentTeacherId,
        CancellationToken cancellationToken = default)
    {
        var query = db.CircleVisitRatings.AsNoTracking();
        if (!isAdmin)
            query = query.Where(x => x.CreatedBy == currentTeacherId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.VisitDate)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CircleVisitRatingListItemDto
            {
                Id = x.Id,
                TeacherName = x.Teacher != null ? x.Teacher.Name ?? "" : "",
                CircleName = x.QuranCircle != null ? x.QuranCircle.Name ?? "" : "",
                VisitDate = x.VisitDate,
                VisitTime = x.VisitTime.ToString(@"hh\:mm"),
                VisitNumberInMonth = x.VisitNumberInMonth,
                CreatedByName = db.Teachers
                    .Where(t => t.Id == x.CreatedBy)
                    .Select(t => t.Name ?? "")
                    .FirstOrDefault() ?? "",
                CreatedAt = x.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<CircleVisitRating?> GetByIdWithItemsAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        db.CircleVisitRatings
            .Include(x => x.Items)
            .Include(x => x.Teacher)
            .Include(x => x.QuranCircle)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<string?> GetTeacherNameAsync(int teacherId, CancellationToken cancellationToken = default) =>
        db.Teachers.AsNoTracking()
            .Where(t => t.Id == teacherId)
            .Select(t => t.Name)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<List<CircleVisitRatingTeacherOptionDto>> GetTeachersAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default) =>
        db.Teachers.AsNoTracking()
            .Where(t => t.IsGirlTeacher == isGirlTeacher && t.UsersManage == false)
            .OrderBy(t => t.Name)
            .Select(t => new CircleVisitRatingTeacherOptionDto { Id = t.Id, Name = t.Name ?? "" })
            .ToListAsync(cancellationToken);

    public Task<List<CircleVisitRatingCircleOptionDto>> GetCirclesForTeacherAsync(
        int teacherId,
        bool isGirlTeacher,
        CancellationToken cancellationToken = default) =>
        db.QuranCircles.AsNoTracking()
            .Where(c => c.TeacherId == teacherId
                && c.Teacher != null
                && c.Teacher.IsGirlTeacher == isGirlTeacher)
            .OrderBy(c => c.Name)
            .Select(c => new CircleVisitRatingCircleOptionDto { Id = c.Id, Name = c.Name ?? "" })
            .ToListAsync(cancellationToken);

    public Task<bool> CircleBelongsToTeacherAsync(
        int circleId,
        int teacherId,
        CancellationToken cancellationToken = default) =>
        db.QuranCircles.AsNoTracking()
            .AnyAsync(c => c.Id == circleId && c.TeacherId == teacherId, cancellationToken);

    public Task<int> CountVisitsForTeacherInMonthAsync(
        int teacherId,
        int year,
        int month,
        CancellationToken cancellationToken = default) =>
        db.CircleVisitRatings.AsNoTracking()
            .CountAsync(
                x => x.TeacherId == teacherId
                    && x.VisitDate.Year == year
                    && x.VisitDate.Month == month,
                cancellationToken);

    public async Task AddAsync(CircleVisitRating entity, CancellationToken cancellationToken = default) =>
        await db.CircleVisitRatings.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
