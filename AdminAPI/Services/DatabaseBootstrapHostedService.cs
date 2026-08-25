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
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
        var ensureDatabase = deploymentOptions.Value.EnsureDatabase;

        for (var attempt = 1; attempt <= 30; attempt++)
        {
            try
            {
                if (ensureDatabase)
                    await db.Database.EnsureCreatedAsync(cancellationToken);

                // Schema patches are always safe/idempotent — run even when EnsureDatabase is false
                // (existing production DBs often skip EnsureCreated but still need new columns).
                await EnsurePrimaryColorColumnAsync(db, cancellationToken);
                await EnsureIntegrationSettingsTableAsync(db, cancellationToken);
                await EnsureCircleVisitRatingsTablesAsync(db, cancellationToken);
                await EnsureTeacherIsSupervisorColumnAsync(db, cancellationToken);
                await EventPageSchemaBootstrap.EnsureTablesAsync(db, cancellationToken);

                if (ensureDatabase)
                    await EnsureDefaultSettingsRowAsync(db, cancellationToken);

                logger.LogInformation(
                    "Database bootstrap completed (EnsureDatabase={EnsureDatabase})",
                    ensureDatabase);
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

    private static async Task EnsurePrimaryColorColumnAsync(AdminDbContext db, CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.MasgedSettings', N'U') IS NOT NULL
               AND COL_LENGTH('MasgedSettings', 'PrimaryColor') IS NULL
                ALTER TABLE MasgedSettings ADD PrimaryColor NVARCHAR(20) NULL;
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

        db.MasgedSettings.Add(new MasgedSetting
        {
            MasgedName = string.Empty,
            PrimaryColor = "#2563eb",
            UpdatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureCircleVisitRatingsTablesAsync(
        AdminDbContext db,
        CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.CircleVisitRatings', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.CircleVisitRatings (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    TeacherId INT NOT NULL,
                    QuranCircleId INT NOT NULL,
                    VisitDate DATE NOT NULL,
                    VisitTime TIME NOT NULL,
                    VisitNumberInMonth INT NOT NULL,
                    CreatedBy INT NOT NULL,
                    CreatedAt DATETIME NOT NULL,
                    CONSTRAINT FK_CircleVisitRatings_Teacher
                        FOREIGN KEY (TeacherId) REFERENCES dbo.Teacher(Id),
                    CONSTRAINT FK_CircleVisitRatings_QuranCircle
                        FOREIGN KEY (QuranCircleId) REFERENCES dbo.QuranCircle(Id)
                );
                CREATE INDEX IX_CircleVisitRatings_TeacherId_VisitDate
                    ON dbo.CircleVisitRatings (TeacherId, VisitDate);
                CREATE INDEX IX_CircleVisitRatings_CreatedBy
                    ON dbo.CircleVisitRatings (CreatedBy);
            END
            """, cancellationToken);

        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.CircleVisitRatingItems', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.CircleVisitRatingItems (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    CircleVisitRatingId INT NOT NULL,
                    Sequence INT NOT NULL,
                    Criterion NVARCHAR(200) NOT NULL,
                    Rating NVARCHAR(50) NOT NULL,
                    Notes NVARCHAR(1000) NULL,
                    CONSTRAINT FK_CircleVisitRatingItems_CircleVisitRatings
                        FOREIGN KEY (CircleVisitRatingId)
                        REFERENCES dbo.CircleVisitRatings(Id) ON DELETE CASCADE
                );
            END
            """, cancellationToken);
    }

    private static async Task EnsureTeacherIsSupervisorColumnAsync(
        AdminDbContext db,
        CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.Teacher', N'U') IS NOT NULL
               AND COL_LENGTH('Teacher', 'IsSupervisor') IS NULL
                ALTER TABLE Teacher ADD IsSupervisor BIT NOT NULL CONSTRAINT DF_Teacher_IsSupervisor DEFAULT 0;
            """, cancellationToken);
    }
}
