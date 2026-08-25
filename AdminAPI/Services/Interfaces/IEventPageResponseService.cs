using AdminAPI.DTOs.EventPageResponses;

namespace AdminAPI.Services.Interfaces;

public interface IEventPageResponseService
{
    Task<EventPageResponsesPageDto> GetListAsync(
        EventPageResponseFiltersDto filters,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportExcelAsync(
        EventPageResponseFiltersDto filters,
        CancellationToken cancellationToken = default);
}
