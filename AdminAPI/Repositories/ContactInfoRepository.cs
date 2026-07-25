using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class ContactInfoRepository(AdminDbContext db) : IContactInfoRepository
{
    private IQueryable<ContactInfo> OrderedQuery() =>
        db.ContactInfos.AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id);

    public async Task<(List<ContactInfo> Items, int TotalCount)> GetPagedAsync(
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

    public Task<ContactInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.ContactInfos.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)
    {
        var hasAny = await db.ContactInfos.AnyAsync(cancellationToken);
        if (!hasAny)
            return 1;

        return await db.ContactInfos.MaxAsync(c => c.SortOrder, cancellationToken) + 1;
    }

    public async Task<ContactInfo> AddAsync(ContactInfo entity, CancellationToken cancellationToken = default)
    {
        await db.ContactInfos.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.ContactInfos.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.ContactInfos.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
