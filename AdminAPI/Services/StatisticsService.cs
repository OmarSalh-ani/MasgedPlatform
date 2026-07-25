using AdminAPI.DTOs.Statistics;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class StatisticsService(
    IStatisticsRepository repository,
    ICurrentUserContext currentUser,
    IWorkDayService workDayService) : IStatisticsService
{
    public async Task<StatisticsResponseDto> GetStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        var today = KuwaitTime.Now.Date;
        var isWorkDay = await workDayService.IsWorkDayAsync(today, cancellationToken);
        return await repository.GetStatisticsAsync(
            currentUser.IsGirlTeacher,
            today,
            isWorkDay,
            cancellationToken);
    }
}
