using AdminAPI.Data;
using AdminAPI.DTOs.Teachers;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class TeacherRepository(AdminDbContext db) : ITeacherRepository
{
    public Task<List<TeacherListItemDto>> GetListAsync(
        bool forGirls,
        CancellationToken cancellationToken = default) =>
        QueryTeachers(forGirls)
            .OrderByDescending(t => t.Id)
            .ToListAsync(cancellationToken);

    public Task<List<TeacherListItemDto>> GetExportListAsync(
        bool forGirls,
        CancellationToken cancellationToken = default) =>
        QueryTeachers(forGirls)
            .OrderByDescending(t => t.Id)
            .ToListAsync(cancellationToken);

    public Task<Teacher?> GetEntityByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Teachers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddEntityAsync(Teacher teacher, CancellationToken cancellationToken = default) =>
        await db.Teachers.AddAsync(teacher, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);

    public async Task<string?> DeleteWithRelatedAsync(
        int id,
        bool forGirls,
        bool restrictCirclesToForGirls,
        CancellationToken cancellationToken = default)
    {
        var teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (teacher is null)
            return null;

        var circlesQuery = db.QuranCircles.Where(c => c.TeacherId == id);
        if (restrictCirclesToForGirls)
            circlesQuery = circlesQuery.Where(c => c.ForGirls == forGirls);

        var circles = await circlesQuery.ToListAsync(cancellationToken);

        foreach (var circle in circles)
        {
            var memorizingCards = await db.StudentMemorizingCards
                .Where(c => c.CircleId == circle.Id)
                .ToListAsync(cancellationToken);
            db.StudentMemorizingCards.RemoveRange(memorizingCards);
        }

        foreach (var circle in circles)
            circle.TeacherId = null;

        var expensivesQuery = db.Expensives.Where(e => e.TeacherId == id);
        if (restrictCirclesToForGirls)
            expensivesQuery = expensivesQuery.Where(e => e.ForGirls == forGirls);

        var expensives = await expensivesQuery.ToListAsync(cancellationToken);
        foreach (var expensive in expensives)
            expensive.TeacherId = null;

        var attendances = await db.CircleAttendances
            .Where(a => a.TeacherId == id)
            .ToListAsync(cancellationToken);
        foreach (var attendance in attendances)
            attendance.TeacherId = null;

        var testHeads = await db.TestHeads
            .Where(t => t.TeacherId == id)
            .ToListAsync(cancellationToken);
        db.TestHeads.RemoveRange(testHeads);

        var teacherLocations = await db.TeacherMapLocations
            .Where(x => x.TeacherId == id)
            .ToListAsync(cancellationToken);
        db.TeacherMapLocations.RemoveRange(teacherLocations);

        var imageFileName = teacher.Image;
        db.Teachers.Remove(teacher);
        await db.SaveChangesAsync(cancellationToken);
        return imageFileName;
    }

    private IQueryable<TeacherListItemDto> QueryTeachers(bool forGirls) =>
        db.Teachers
            .AsNoTracking()
            .Where(t => t.IsGirlTeacher == forGirls)
            .Select(t => new TeacherListItemDto
            {
                Id = t.Id,
                Name = t.Name,
                CircleCount = db.QuranCircles.Count(c => c.TeacherId == t.Id),
                Mobile = t.Mobile,
                Email = t.Email,
                Password = t.Password,
                UsersManage = t.UsersManage,
                ImageUrl = string.IsNullOrEmpty(t.Image) ? null : t.Image,
            });
}
