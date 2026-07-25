using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.PlanLevels;

namespace AdminAPI.Services.Interfaces;

public interface IPlanLevelService
{
    Task<PagedResultDto<PlanLevelListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PlanLevelDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PlanLevelDto> CreateAsync(
        SavePlanLevelRequestDto request,
        CancellationToken cancellationToken = default);

    Task<PlanLevelDto> UpdateAsync(
        int id,
        SavePlanLevelRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
