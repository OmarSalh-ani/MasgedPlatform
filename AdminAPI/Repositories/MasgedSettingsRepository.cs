using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class MasgedSettingsRepository(AdminDbContext db) : IMasgedSettingsRepository
{
    public Task<MasgedSetting?> GetFirstAsync(CancellationToken cancellationToken = default) =>
        db.MasgedSettings.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public async Task<MasgedSetting> AddAsync(MasgedSetting entity, CancellationToken cancellationToken = default)
    {
        await db.MasgedSettings.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
