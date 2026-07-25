using AdminAPI.DTOs.Activities;
using AdminAPI.DTOs.Activity;
using AdminAPI.DTOs.Common;

namespace AdminAPI.Services.Interfaces;

public interface IActivityService
{
    Task<PagedResultDto<ActivityListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ActivityDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<ActivityDto> CreateAsync(SaveActivityRequestDto request, CancellationToken cancellationToken = default);
    Task<ActivityDto> UpdateAsync(int id, SaveActivityRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
