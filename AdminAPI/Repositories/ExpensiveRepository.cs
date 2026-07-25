using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class ExpensiveRepository(AdminDbContext db) : IExpensiveRepository
{
    public Task<List<Expensive>> GetListAsync(bool forGirls, CancellationToken cancellationToken = default) =>
        db.Expensives
            .AsNoTracking()
            .Include(e => e.Teacher)
            .Where(e => e.ForGirls == forGirls)
            .OrderByDescending(e => e.Id)
            .ToListAsync(cancellationToken);

    public Task<Expensive?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Expensives.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Expensive?> GetByIdScopedAsync(int id, bool forGirls, CancellationToken cancellationToken = default) =>
        db.Expensives.FirstOrDefaultAsync(e => e.Id == id && e.ForGirls == forGirls, cancellationToken);

    public async Task<Expensive> AddAsync(Expensive entity, CancellationToken cancellationToken = default)
    {
        await db.Expensives.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, bool forGirls, CancellationToken cancellationToken = default)
    {
        var entity = await db.Expensives.FirstOrDefaultAsync(
            e => e.Id == id && e.ForGirls == forGirls,
            cancellationToken);
        if (entity is null)
            return false;

        db.Expensives.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
