using AdminAPI.DTOs.EventPageResponses;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class EventPageResponseService(
    IEventPageResponseRepository repository) : IEventPageResponseService
{
    public async Task<EventPageResponsesPageDto> GetListAsync(
        EventPageResponseFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = filters.PageNumber < 1 ? 1 : filters.PageNumber;
        var pageSize = filters.PageSize <= 0 ? 20 : filters.PageSize;
        var activityName = string.IsNullOrWhiteSpace(filters.ActivityName)
            ? null
            : filters.ActivityName.Trim();

        var (items, totalCount) = await repository.GetPagedAsync(
            activityName,
            pageNumber,
            pageSize,
            cancellationToken);

        var fieldLabels = await repository.GetFieldLabelsAsync(activityName, cancellationToken);

        return new EventPageResponsesPageDto
        {
            Items = items.Select(MapItem).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            FieldLabels = fieldLabels,
        };
    }

    public async Task<byte[]> ExportExcelAsync(
        EventPageResponseFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var activityName = string.IsNullOrWhiteSpace(filters.ActivityName)
            ? null
            : filters.ActivityName.Trim();

        var items = await repository.GetForExportAsync(activityName, cancellationToken);
        var fieldLabels = await repository.GetFieldLabelsAsync(activityName, cancellationToken);
        return EventPageResponseExcelExporter.Build(items.Select(MapItem).ToList(), fieldLabels);
    }

    private static EventPageResponseListItemDto MapItem(EventPageResponse response) => new()
    {
        Id = response.Id,
        EventPageId = response.EventPageId,
        ActivityName = response.ActivityName,
        SubmittedAt = response.SubmittedAt,
        Values = response.Values
            .Select(v => new EventPageResponseValueDto
            {
                FieldLabel = v.FieldLabel,
                Value = v.Value,
            })
            .ToList(),
    };
}
