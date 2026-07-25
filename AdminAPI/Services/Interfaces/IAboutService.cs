using AdminAPI.DTOs.About;

namespace AdminAPI.Services.Interfaces;

public interface IAboutService
{
    Task<AboutDto?> GetAsync(CancellationToken cancellationToken = default);
    Task<AboutDto> SaveAsync(UpdateAboutRequestDto request, CancellationToken cancellationToken = default);
}
