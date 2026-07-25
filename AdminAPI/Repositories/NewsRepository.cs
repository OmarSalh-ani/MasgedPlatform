using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class NewsRepository(AdminDbContext db) : INewsRepository
{
    private IQueryable<NewsItem> OrderedQuery() =>
        db.NewsItems.AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.NewsDate);

    public async Task<(List<NewsItem> Items, int TotalCount)> GetPagedAsync(
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

    public Task<NewsItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.NewsItems.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var hasAny = await db.NewsItems.AnyAsync(cancellationToken);
        if (!hasAny)
            return 1;

        return await db.NewsItems.MaxAsync(n => n.SortOrder, cancellationToken) + 1;
    }

    public async Task<NewsItem> AddAsync(NewsItem entity, CancellationToken cancellationToken = default)
    {
        await db.NewsItems.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.NewsItems.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.NewsItems.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
