using AdminAPI.DTOs.QuranCircles;

namespace AdminAPI.Services.Interfaces;

public interface IQuranCirclePlansClearService
{
    Task DeletePlansAsync(DeleteCirclePlansRequestDto request, CancellationToken cancellationToken = default);
}
