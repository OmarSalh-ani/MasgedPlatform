using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class HeroSlideRepository(AdminDbContext db) : IHeroSlideRepository
{
    private IQueryable<HeroSlide> OrderedQuery() =>
        db.HeroSlides.AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id);

    public async Task<(List<HeroSlide> Items, int TotalCount)> GetPagedAsync(
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

    public Task<HeroSlide?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.HeroSlides.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var hasAny = await db.HeroSlides.AnyAsync(cancellationToken);
        if (!hasAny)
            return 1;

        return await db.HeroSlides.MaxAsync(x => x.SortOrder, cancellationToken) + 1;
    }

    public async Task<HeroSlide> AddAsync(HeroSlide entity, CancellationToken cancellationToken = default)
    {
        await db.HeroSlides.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.HeroSlides.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.HeroSlides.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
