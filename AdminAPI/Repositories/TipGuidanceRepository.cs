using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class TipGuidanceRepository(AdminDbContext db) : ITipGuidanceRepository
{
    public async Task<(List<TipGuidance> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = db.TipGuidances.AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<TipGuidance?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.TipGuidances.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        if (!await db.TipGuidances.AnyAsync(cancellationToken))
            return 1;

        return await db.TipGuidances.MaxAsync(x => x.SortOrder, cancellationToken) + 1;
    }

    public async Task<TipGuidance> AddAsync(TipGuidance entity, CancellationToken cancellationToken = default)
    {
        await db.TipGuidances.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.TipGuidances.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.TipGuidances.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
