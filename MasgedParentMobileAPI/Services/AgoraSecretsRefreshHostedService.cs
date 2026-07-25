using MasgedParentMobileAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Services;

public class AgoraSecretsRefreshHostedService(
    IServiceScopeFactory scopeFactory,
    AgoraSecretsCache cache,
    ILogger<AgoraSecretsRefreshHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<NewMasgedTeacherAPIDBContext>();
                var row = await db.IntegrationSettings.AsNoTracking()
                    .FirstOrDefaultAsync(stoppingToken);
                cache.Replace(row?.AgoraAppId, row?.AgoraAppCertificate);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Agora secrets refresh skipped");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
