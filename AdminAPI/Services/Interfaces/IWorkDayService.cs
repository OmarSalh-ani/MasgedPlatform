namespace AdminAPI.Services.Interfaces;

public interface IWorkDayService
{
    Task<DTOs.WorkDays.WorkDaysDto> GetAsync(CancellationToken cancellationToken = default);

    Task<DTOs.WorkDays.WorkDaysDto> UpdateAsync(
        DTOs.WorkDays.UpdateWorkDaysRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetWorkDayNumbersAsync(CancellationToken cancellationToken = default);

    Task<bool> IsWorkDayAsync(DateTime date, CancellationToken cancellationToken = default);

    Task<int> CountWorkDaysInRangeAsync(
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default);

    Task<DateTime> GetNextWorkDayAsync(DateTime from, CancellationToken cancellationToken = default);
}
