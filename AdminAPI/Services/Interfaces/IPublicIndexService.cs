namespace AdminAPI.Services.Interfaces;

using AdminAPI.DTOs.PublicIndex;

public interface IPublicIndexService
{
    Task<PublicWebsiteContentDto> GetWebsiteContentAsync(CancellationToken cancellationToken = default);
    Task<PublicRegistrationConfigDto> GetRegistrationConfigAsync(
        string? mode,
        CancellationToken cancellationToken = default);
    Task<SubmitPublicRegistrationResponseDto> SubmitRegistrationAsync(
        SubmitPublicRegistrationRequestDto request,
        CancellationToken cancellationToken = default);
    Task<PublicRegisterSuccessDto> GetRegisterSuccessAsync(CancellationToken cancellationToken = default);
}
