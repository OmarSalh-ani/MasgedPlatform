using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class ActivityRepository(AdminDbContext db) : IActivityRepository
{
    private IQueryable<Activity> OrderedQuery() =>
        db.Activities.AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.Id);

    public async Task<(List<Activity> Items, int TotalCount)> GetPagedAsync(
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

    public Task<Activity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Activities.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var hasAny = await db.Activities.AnyAsync(cancellationToken);
        if (!hasAny)
            return 1;

        return await db.Activities.MaxAsync(a => a.SortOrder, cancellationToken) + 1;
    }

    public async Task<Activity> AddAsync(Activity entity, CancellationToken cancellationToken = default)
    {
        await db.Activities.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Activities.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.Activities.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
