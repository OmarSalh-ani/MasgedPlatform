using AdminAPI.DTOs.PublicEventPages;

namespace AdminAPI.Services.Interfaces;

public interface IPublicEventPageService
{
    Task<PublicEventPageDto> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task SubmitRegistrationAsync(
        string slug,
        SubmitEventPageRegistrationRequestDto request,
        CancellationToken cancellationToken = default);
}
