using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class SocialLinkRepository(AdminDbContext db) : ISocialLinkRepository
{
    private IQueryable<SocialLink> OrderedQuery() =>
        db.SocialLinks.AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id);

    public async Task<(List<SocialLink> Items, int TotalCount)> GetPagedAsync(
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

    public Task<SocialLink?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.SocialLinks.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var hasAny = await db.SocialLinks.AnyAsync(cancellationToken);
        if (!hasAny)
            return 1;

        return await db.SocialLinks.MaxAsync(s => s.SortOrder, cancellationToken) + 1;
    }

    public async Task<SocialLink> AddAsync(SocialLink entity, CancellationToken cancellationToken = default)
    {
        await db.SocialLinks.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.SocialLinks.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.SocialLinks.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
