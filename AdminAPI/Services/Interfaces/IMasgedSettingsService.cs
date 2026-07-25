using AdminAPI.DTOs.MasgedSettings;

namespace AdminAPI.Services.Interfaces;

public interface IMasgedSettingsService
{
    Task<MasgedSettingsDto?> GetAsync(CancellationToken cancellationToken = default);
    Task<SetupStatusDto> GetSetupStatusAsync(CancellationToken cancellationToken = default);
    Task<MasgedSettingsDto> CompleteSetupAsync(
        FirstTimeSetupRequestDto request,
        CancellationToken cancellationToken = default);
    Task<MasgedSettingsDto> SaveAsync(
        UpdateMasgedSettingsRequestDto request,
        CancellationToken cancellationToken = default);
}
