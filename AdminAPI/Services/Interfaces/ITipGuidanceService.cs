using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TipGuidance;

namespace AdminAPI.Services.Interfaces;

public interface ITipGuidanceService
{
    Task<PagedResultDto<TipGuidanceListItemDto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<TipGuidanceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<TipGuidanceDto> CreateAsync(
        SaveTipGuidanceRequestDto request,
        CancellationToken cancellationToken = default);

    Task<TipGuidanceDto> UpdateAsync(
        int id,
        SaveTipGuidanceRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        bool deleteImageFile,
        CancellationToken cancellationToken = default);
}
