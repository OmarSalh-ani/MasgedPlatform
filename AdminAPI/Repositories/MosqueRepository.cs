using AdminAPI.Data;

using AdminAPI.Models;

using AdminAPI.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;



namespace AdminAPI.Repositories;



public class MosqueRepository(AdminDbContext db) : IMosqueRepository

{

    private IQueryable<Mosque> OrderedQuery() =>

        db.Mosques.AsNoTracking()

            .OrderBy(x => x.SortOrder)

            .ThenByDescending(x => x.Id);



    public async Task<(List<Mosque> Items, int TotalCount)> GetPagedAsync(

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



    public Task<Mosque?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>

        db.Mosques.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);



    public async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default)

    {

        var hasAny = await db.Mosques.AnyAsync(cancellationToken);

        if (!hasAny)

            return 1;



        return await db.Mosques.MaxAsync(m => m.SortOrder, cancellationToken) + 1;

    }



    public async Task<Mosque> AddAsync(Mosque entity, CancellationToken cancellationToken = default)

    {

        await db.Mosques.AddAsync(entity, cancellationToken);

        return entity;

    }



    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)

    {

        var entity = await db.Mosques.FindAsync([id], cancellationToken);

        if (entity is null)

            return false;



        db.Mosques.Remove(entity);

        return true;

    }



    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>

        db.SaveChangesAsync(cancellationToken);

}

