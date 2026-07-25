using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class PlanLevelRepository(AdminDbContext db) : IPlanLevelRepository
{
    private IQueryable<PlanLevel> OrderedQuery() =>
        db.PlanLevels.AsNoTracking().OrderByDescending(x => x.Id);

    public async Task<(List<PlanLevel> Items, int TotalCount)> GetPagedAsync(
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

    public Task<PlanLevel?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.PlanLevels.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> HasReadyPlanDependencyAsync(int id, CancellationToken cancellationToken = default) =>
        db.ReadyPlans.AnyAsync(x => x.PlanLevelId == id, cancellationToken);

    public Task<bool> HasRegisterFormDependencyAsync(int id, CancellationToken cancellationToken = default) =>
        db.RegisterForms.AnyAsync(x => x.PlanLevelId == id, cancellationToken);

    public async Task<PlanLevel> AddAsync(PlanLevel entity, CancellationToken cancellationToken = default)
    {
        await db.PlanLevels.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.PlanLevels.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.PlanLevels.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
