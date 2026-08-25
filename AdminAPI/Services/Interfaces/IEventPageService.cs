using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.EventPages;

namespace AdminAPI.Services.Interfaces;

public interface IEventPageService
{
    Task<PagedResultDto<EventPageListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<EventPageDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<EventPageLookupDto>> GetLookupsAsync(CancellationToken cancellationToken = default);
    Task<EventPageDto> CreateAsync(SaveEventPageRequestDto request, CancellationToken cancellationToken = default);
    Task<EventPageDto> UpdateAsync(int id, SaveEventPageRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
