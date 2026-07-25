using AdminAPI.DTOs.Integrations;

namespace AdminAPI.Services.Interfaces;

public interface IIntegrationSettingsService
{
    Task<IntegrationSettingsDto> GetAsync(CancellationToken cancellationToken = default);
    Task<IntegrationSettingsDto> SaveAsync(
        UpdateIntegrationSettingsRequestDto request,
        CancellationToken cancellationToken = default);
}
