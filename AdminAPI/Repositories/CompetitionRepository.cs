using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class CompetitionRepository(AdminDbContext db) : ICompetitionRepository
{
    public async Task<(List<Competition> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = db.Competitions.AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Competition?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Competitions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        if (!await db.Competitions.AnyAsync(cancellationToken))
            return 1;

        return await db.Competitions.MaxAsync(x => x.SortOrder, cancellationToken) + 1;
    }

    public async Task<Competition> AddAsync(Competition entity, CancellationToken cancellationToken = default)
    {
        await db.Competitions.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Competitions.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.Competitions.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
