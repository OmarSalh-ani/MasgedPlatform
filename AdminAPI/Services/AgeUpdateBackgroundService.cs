using AdminAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

/// <summary>
/// Runs student age sync once per Kuwait calendar day at a configured local time.
/// </summary>
public class AgeUpdateBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AgeUpdateOptions _options;
    private readonly AgeUpdateLastRunState _lastRunState;
    private readonly ILogger<AgeUpdateBackgroundService> _logger;

    public AgeUpdateBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<AgeUpdateOptions> options,
        AgeUpdateLastRunState lastRunState,
        ILogger<AgeUpdateBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _lastRunState = lastRunState;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Age update background job started. Runs daily at {Hour:D2}:{Minute:D2} Kuwait time.",
            _options.RunAtHour,
            _options.RunAtMinute);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_lastRunState.HasRunToday())
            {
                var scheduledToday = GetScheduledTime(KuwaitTime.Today);
                var now = KuwaitTime.Now;

                if (now < scheduledToday)
                {
                    var waitUntilScheduled = scheduledToday - now;
                    _logger.LogInformation(
                        "Age update scheduled for {ScheduledAt:yyyy-MM-dd HH:mm} Kuwait time (in {Delay}).",
                        scheduledToday,
                        waitUntilScheduled);

                    await Task.Delay(waitUntilScheduled, stoppingToken);
                }

                if (!_lastRunState.HasRunToday())
                    await RunAgeUpdateAsync(stoppingToken);
            }

            var nextScheduled = GetScheduledTime(KuwaitTime.Today.AddDays(1));
            var delayUntilNext = nextScheduled - KuwaitTime.Now;

            if (delayUntilNext > TimeSpan.Zero)
            {
                _logger.LogInformation(
                    "Next age update at {NextRun:yyyy-MM-dd HH:mm} Kuwait time (in {Delay}).",
                    nextScheduled,
                    delayUntilNext);

                await Task.Delay(delayUntilNext, stoppingToken);
            }
        }
    }

    private async Task RunAgeUpdateAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var ageUpdateService = scope.ServiceProvider.GetRequiredService<IAgeUpdateService>();
            await ageUpdateService.UpdateAgesIfNeededAsync(cancellationToken);
            _lastRunState.MarkRunToday();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Age update background job failed");
        }
    }

    private DateTime GetScheduledTime(DateTime kuwaitDate)
    {
        var hour = Math.Clamp(_options.RunAtHour, 0, 23);
        var minute = Math.Clamp(_options.RunAtMinute, 0, 59);
        return kuwaitDate.Date.AddHours(hour).AddMinutes(minute);
    }
}
