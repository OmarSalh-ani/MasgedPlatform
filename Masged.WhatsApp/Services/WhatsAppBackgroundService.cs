using Masged.WhatsApp.Interfaces;
using Masged.WhatsApp.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masged.WhatsApp.Services;

/// <summary>
/// Sends WhatsApp messages from a queue repository using WasenderAPI.
/// Failed messages are appended to a text log file.
/// </summary>
public class WhatsAppBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWasenderApiClient _wasenderApiClient;
    private readonly WhatsAppProcessorOptions _options;
    private readonly ILogger<WhatsAppBackgroundService> _logger;
    private readonly SemaphoreSlim _processLock = new(1, 1);
    private readonly object _logLock = new();

    public WhatsAppBackgroundService(
        IServiceScopeFactory scopeFactory,
        IWasenderApiClient wasenderApiClient,
        IOptions<WhatsAppProcessorOptions> options,
        ILogger<WhatsAppBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _wasenderApiClient = wasenderApiClient;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "WhatsApp background processor started. Interval: {IntervalSeconds}s",
            _options.IntervalSeconds);

        if (_options.InitialDelaySeconds > 0)
            await Task.Delay(TimeSpan.FromSeconds(_options.InitialDelaySeconds), stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.IntervalSeconds));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (!await _processLock.WaitAsync(0, stoppingToken))
                continue;

            try
            {
                await ProcessQueueAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                LogError("ProcessPendingMessages", "", "", ex.ToString());
            }
            finally
            {
                _processLock.Release();
            }
        }
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<IWhatsappQueueRepository>();
        var sessionKeySync = scope.ServiceProvider.GetRequiredService<WasenderSessionKeySyncService>();

        try
        {
            await sessionKeySync.EnsureSessionApiKeyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("EnsureSessionApiKey", "", "", ex.ToString());
        }

        var batch = await queue.DequeueBatchAsync(_options.BatchSize, cancellationToken);

        foreach (var row in batch)
        {
            try
            {
                var result = await _wasenderApiClient.SendMessageAsync(
                    row.Mobile ?? "",
                    row.Message ?? "",
                    row.Image,
                    cancellationToken);

                if (result.Success)
                {
                    await queue.RemoveAsync(row.Id, cancellationToken);
                }
                else
                {
                    LogError(
                        $"queue id={row.Id} DROPPED",
                        row.Mobile,
                        row.Message,
                        result.Error ?? "Unknown error");

                    await queue.RemoveAsync(row.Id, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                LogError(
                    $"queue id={row.Id} EXCEPTION DROPPED",
                    row.Mobile,
                    row.Message,
                    ex.ToString());

                await queue.RemoveAsync(row.Id, cancellationToken);
            }

            if (_options.DelayBetweenMessagesMs > 0)
                await Task.Delay(_options.DelayBetweenMessagesMs, cancellationToken);
        }
    }

    private void LogError(string context, string? mobile, string? message, string error)
    {
        try
        {
            lock (_logLock)
            {
                var directory = Path.Combine(AppContext.BaseDirectory, "Logs");
                Directory.CreateDirectory(directory);
                var path = Path.Combine(directory, _options.ErrorLogFileName);

                var logLine =
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                    $"Context: {context} | " +
                    $"Mobile: {mobile} | " +
                    $"Message: {message} | " +
                    $"Error: {error}";

                File.AppendAllText(path, logLine + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write WhatsApp error log");
        }
    }
}
