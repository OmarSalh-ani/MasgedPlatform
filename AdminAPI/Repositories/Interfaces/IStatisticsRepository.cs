using AdminAPI.DTOs.Statistics;

namespace AdminAPI.Repositories.Interfaces;

public interface IStatisticsRepository
{
    Task<StatisticsResponseDto> GetStatisticsAsync(
        bool isGirlTeacher,
        DateTime today,
        bool isWorkDay,
        CancellationToken cancellationToken = default);
}
