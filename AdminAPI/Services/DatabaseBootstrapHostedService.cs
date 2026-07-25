using AdminAPI.Configuration;
using AdminAPI.Data;
using AdminAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class DatabaseBootstrapHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<DeploymentOptions> deploymentOptions,
    ILogger<DatabaseBootstrapHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!deploymentOptions.Value.EnsureDatabase)
            return;

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();

        for (var attempt = 1; attempt <= 30; attempt++)
        {
            try
            {
                await db.Database.EnsureCreatedAsync(cancellationToken);
                await EnsureWhiteLabelColumnsAsync(db, cancellationToken);
                await EnsureIntegrationSettingsTableAsync(db, cancellationToken);
                await EnsureDefaultSettingsRowAsync(db, cancellationToken);
                logger.LogInformation("Database bootstrap completed");
                return;
            }
            catch (Exception ex) when (attempt < 30)
            {
                logger.LogWarning(ex, "Database bootstrap attempt {Attempt} failed; retrying", attempt);
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task EnsureWhiteLabelColumnsAsync(AdminDbContext db, CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('MasgedSettings', 'PrimaryColor') IS NULL
                ALTER TABLE MasgedSettings ADD PrimaryColor NVARCHAR(20) NULL;
            """, cancellationToken);
        await db.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('MasgedSettings', 'Domain') IS NULL
                ALTER TABLE MasgedSettings ADD Domain NVARCHAR(200) NULL;
            """, cancellationToken);
        await db.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('MasgedSettings', 'SetupCompleted') IS NULL
            BEGIN
                ALTER TABLE MasgedSettings ADD SetupCompleted BIT NOT NULL CONSTRAINT DF_MasgedSettings_SetupCompleted DEFAULT (0);
                UPDATE MasgedSettings SET SetupCompleted = 1 WHERE LEN(LTRIM(RTRIM(ISNULL(MasgedName, N'')))) > 0;
            END
            """, cancellationToken);
    }

    private static async Task EnsureIntegrationSettingsTableAsync(
        AdminDbContext db,
        CancellationToken cancellationToken)
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

    private async Task EnsureDefaultSettingsRowAsync(AdminDbContext db, CancellationToken cancellationToken)
    {
        if (await db.MasgedSettings.AnyAsync(cancellationToken))
            return;

        var domain = deploymentOptions.Value.Domain?.Trim() ?? string.Empty;
        db.MasgedSettings.Add(new MasgedSetting
        {
            MasgedName = string.Empty,
            PrimaryColor = "#2563eb",
            Domain = string.IsNullOrWhiteSpace(domain) ? null : domain,
            SetupCompleted = false,
            UpdatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}
