namespace MasgedParentMobileAPI.Services;

public interface IWorkDayService
{
    Task<IReadOnlyList<int>> GetWorkDayNumbersAsync(CancellationToken cancellationToken = default);

    Task<bool> IsWorkDayAsync(DateTime date, CancellationToken cancellationToken = default);

    Task<int> CountWorkDaysInRangeAsync(
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default);

    Task<DateTime> GetNextWorkDayAsync(DateTime from, CancellationToken cancellationToken = default);
}
