using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class FilesManagerRepository(AdminDbContext db) : IFilesManagerRepository
{
    private IQueryable<FilesManager> OrderedQuery() =>
        db.FilesManagers.AsNoTracking().OrderByDescending(x => x.Id);

    public async Task<(List<FilesManager> Items, int TotalCount)> GetPagedAsync(
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

    public Task<FilesManager?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.FilesManagers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<FilesManager> AddAsync(FilesManager entity, CancellationToken cancellationToken = default)
    {
        await db.FilesManagers.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.FilesManagers.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.FilesManagers.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
