using AdminAPI.DTOs.Subscribe;

namespace AdminAPI.Services.Interfaces;

public interface ISubscribeService
{
    Task<SubmitSubscribeResponseDto> SubmitAsync(
        SubmitSubscribeRequestDto request,
        CancellationToken cancellationToken = default);
}
