using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IMasgedSettingsRepository
{
    Task<MasgedSetting?> GetFirstAsync(CancellationToken cancellationToken = default);
    Task<MasgedSetting> AddAsync(MasgedSetting entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
