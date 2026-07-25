using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.WomansActivities;

namespace AdminAPI.Services.Interfaces;

public interface IWomansActivityService
{
    Task<PagedResultDto<WomanActivityListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<WomanActivityDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<WomanActivityDto> CreateAsync(
        SaveWomanActivityRequestDto request,
        CancellationToken cancellationToken = default);

    Task<WomanActivityDto> UpdateAsync(
        int id,
        SaveWomanActivityRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default);
}
