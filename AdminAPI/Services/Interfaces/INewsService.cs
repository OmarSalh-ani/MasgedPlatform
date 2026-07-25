using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.News;

namespace AdminAPI.Services.Interfaces;

public interface INewsService
{
    Task<PagedResultDto<NewsListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<NewsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<NewsDto> CreateAsync(SaveNewsRequestDto request, CancellationToken cancellationToken = default);
    Task<NewsDto> UpdateAsync(int id, SaveNewsRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
