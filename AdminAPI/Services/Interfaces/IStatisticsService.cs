using AdminAPI.DTOs.Statistics;

namespace AdminAPI.Services.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsResponseDto> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
