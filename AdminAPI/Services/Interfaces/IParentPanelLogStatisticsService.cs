using AdminAPI.DTOs.ParentPanelLogStatistics;

namespace AdminAPI.Services.Interfaces;

public interface IParentPanelLogStatisticsService
{
    Task<ParentPanelLogStatisticsResponseDto> GetStatisticsAsync(
        string? fromDate,
        string? toDate,
        CancellationToken cancellationToken = default);
}
