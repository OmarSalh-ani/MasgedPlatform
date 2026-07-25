using AdminAPI.Data;
using AdminAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public class IntegrationSecretsBootstrapHostedService(
    IServiceScopeFactory scopeFactory,
    IntegrationSecretsCache cache,
    ILogger<IntegrationSecretsBootstrapHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
            await EnsureTableAsync(db, cancellationToken);
            var row = await db.IntegrationSettings.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (row is null)
                return;

            cache.Replace(
                row.WasenderApiToken,
                row.WasenderSessionApiKey,
                row.AgoraAppId,
                row.AgoraAppCertificate);
            logger.LogInformation("Loaded integration secrets overrides from database");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not load integration secrets overrides");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task EnsureTableAsync(AdminDbContext db, CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.IntegrationSettings', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.IntegrationSettings (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    WasenderApiToken NVARCHAR(500) NULL,
                    WasenderSessionApiKey NVARCHAR(500) NULL,
                    AgoraAppId NVARCHAR(200) NULL,
                    AgoraAppCertificate NVARCHAR(200) NULL,
                    UpdatedAt DATETIME NULL
                );
            END
            """, cancellationToken);
    }
}
