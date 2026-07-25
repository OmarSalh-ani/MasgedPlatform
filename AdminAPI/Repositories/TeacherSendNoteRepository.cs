using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class TeacherSendNoteRepository(AdminDbContext db) : ITeacherSendNoteRepository
{
    private IQueryable<TeachersAdminNote> OrderedQuery() =>
        db.TeachersAdminNotes.AsNoTracking()
            .Include(n => n.Teacher)
            .OrderByDescending(n => n.CreatedAt);

    public async Task<(List<TeachersAdminNote> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = OrderedQuery();
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<TeachersAdminNote?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.TeachersAdminNotes
            .Include(n => n.Teacher)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public Task<List<Teacher>> GetTeachersOrderedByNameAsync(
        CancellationToken cancellationToken = default) =>
        db.Teachers.AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public async Task AddRangeAsync(
        IEnumerable<TeachersAdminNote> notes,
        CancellationToken cancellationToken = default) =>
        await db.TeachersAdminNotes.AddRangeAsync(notes, cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.TeachersAdminNotes.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.TeachersAdminNotes.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
