using AdminAPI.Data;
using AdminAPI.DTOs.WomansActivities;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class WomansActivityRepository(AdminDbContext db) : IWomansActivityRepository
{
    public Task<List<WomanActivityListItemDto>> GetListAsync(
        bool forGirl,
        CancellationToken cancellationToken = default) =>
        db.WomanActivities
            .AsNoTracking()
            .Where(x => x.ForGirl == forGirl)
            .OrderByDescending(x => x.Id)
            .Select(x => new WomanActivityListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                IsVisible = x.IsVisible,
            })
            .ToListAsync(cancellationToken);

    public Task<WomanActivity?> GetByIdAsync(
        int id,
        bool forGirl,
        CancellationToken cancellationToken = default) =>
        db.WomanActivities.FirstOrDefaultAsync(
            x => x.Id == id && x.ForGirl == forGirl,
            cancellationToken);

    public async Task<WomanActivity> AddAsync(
        WomanActivity entity,
        CancellationToken cancellationToken = default)
    {
        await db.WomanActivities.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
