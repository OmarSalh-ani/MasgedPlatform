using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class EventPageRepository(AdminDbContext db) : IEventPageRepository
{
    private IQueryable<EventPage> OrderedQuery() =>
        db.EventPages.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id);

    public async Task<(List<EventPage> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = OrderedQuery();
        var totalCount = await query.CountAsync(cancellationToken);

        if (pageSize <= 0)
        {
            var all = await query.ToListAsync(cancellationToken);
            return (all, totalCount);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<EventPage?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.EventPages
            .Include(p => p.Tracks)
            .Include(p => p.FormFields)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<EventPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        db.EventPages
            .Include(p => p.Tracks)
            .Include(p => p.FormFields)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

    public Task<List<EventPage>> GetLookupsAsync(CancellationToken cancellationToken = default) =>
        db.EventPages.AsNoTracking()
            .OrderBy(p => p.ActivityName)
            .ToListAsync(cancellationToken);

    public Task<bool> ActivityNameExistsAsync(
        string activityName,
        int? excludeId,
        CancellationToken cancellationToken = default) =>
        db.EventPages.AnyAsync(
            p => p.ActivityName == activityName && (!excludeId.HasValue || p.Id != excludeId),
            cancellationToken);

    public Task<bool> SlugExistsAsync(
        string slug,
        int? excludeId,
        CancellationToken cancellationToken = default) =>
        db.EventPages.AnyAsync(
            p => p.Slug == slug && (!excludeId.HasValue || p.Id != excludeId),
            cancellationToken);

    public async Task<EventPage> AddAsync(EventPage entity, CancellationToken cancellationToken = default)
    {
        await db.EventPages.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.EventPages.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.EventPages.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
