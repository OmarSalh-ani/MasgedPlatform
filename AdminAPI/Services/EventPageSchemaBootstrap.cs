using AdminAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public static class EventPageSchemaBootstrap
{
    public static async Task EnsureTablesAsync(AdminDbContext db, CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.EventPages', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.EventPages (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    ActivityName NVARCHAR(200) NOT NULL,
                    Slug NVARCHAR(120) NOT NULL,
                    CourseTitle NVARCHAR(300) NOT NULL,
                    InvitationText NVARCHAR(500) NULL,
                    MosqueName NVARCHAR(300) NULL,
                    SubjectText NVARCHAR(1000) NULL,
                    DateText NVARCHAR(300) NULL,
                    TimeText NVARCHAR(300) NULL,
                    ExtraNotes NVARCHAR(MAX) NULL,
                    SupervisorsText NVARCHAR(1000) NULL,
                    ContactPhone NVARCHAR(50) NULL,
                    SocialAccounts NVARCHAR(200) NULL,
                    LocationNote NVARCHAR(500) NULL,
                    ImageUrl NVARCHAR(500) NULL,
                    IsPublished BIT NOT NULL CONSTRAINT DF_EventPages_IsPublished DEFAULT 1,
                    IsRegistrationOpen BIT NOT NULL CONSTRAINT DF_EventPages_IsRegistrationOpen DEFAULT 1,
                    CreatedAt DATETIME NOT NULL,
                    CONSTRAINT UQ_EventPages_ActivityName UNIQUE (ActivityName),
                    CONSTRAINT UQ_EventPages_Slug UNIQUE (Slug)
                );
            END
            """, cancellationToken);

        await EnsureChildTablesAsync(db, cancellationToken);
    }

    private static async Task EnsureChildTablesAsync(AdminDbContext db, CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.EventPageTracks', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.EventPageTracks (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    EventPageId INT NOT NULL,
                    Title NVARCHAR(300) NOT NULL,
                    Description NVARCHAR(MAX) NULL,
                    SortOrder INT NOT NULL,
                    CONSTRAINT FK_EventPageTracks_EventPages
                        FOREIGN KEY (EventPageId) REFERENCES dbo.EventPages(Id) ON DELETE CASCADE
                );
            END
            """, cancellationToken);

        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.EventPageFormFields', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.EventPageFormFields (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    EventPageId INT NOT NULL,
                    Label NVARCHAR(300) NOT NULL,
                    FieldType NVARCHAR(30) NOT NULL,
                    IsRequired BIT NOT NULL CONSTRAINT DF_EventPageFormFields_IsRequired DEFAULT 0,
                    SortOrder INT NOT NULL,
                    OptionsJson NVARCHAR(MAX) NULL,
                    CONSTRAINT FK_EventPageFormFields_EventPages
                        FOREIGN KEY (EventPageId) REFERENCES dbo.EventPages(Id) ON DELETE CASCADE
                );
            END
            """, cancellationToken);

        await EnsureResponseTablesAsync(db, cancellationToken);
    }

    private static async Task EnsureResponseTablesAsync(AdminDbContext db, CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.EventPageResponses', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.EventPageResponses (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    EventPageId INT NOT NULL,
                    ActivityName NVARCHAR(200) NOT NULL,
                    SubmittedAt DATETIME NOT NULL,
                    CONSTRAINT FK_EventPageResponses_EventPages
                        FOREIGN KEY (EventPageId) REFERENCES dbo.EventPages(Id) ON DELETE CASCADE
                );
                CREATE INDEX IX_EventPageResponses_ActivityName
                    ON dbo.EventPageResponses (ActivityName);
                CREATE INDEX IX_EventPageResponses_SubmittedAt
                    ON dbo.EventPageResponses (SubmittedAt);
            END
            """, cancellationToken);

        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.EventPageResponseValues', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.EventPageResponseValues (
                    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    ResponseId INT NOT NULL,
                    FieldId INT NULL,
                    FieldLabel NVARCHAR(300) NOT NULL,
                    Value NVARCHAR(MAX) NOT NULL,
                    CONSTRAINT FK_EventPageResponseValues_Responses
                        FOREIGN KEY (ResponseId) REFERENCES dbo.EventPageResponses(Id) ON DELETE CASCADE
                );
            END
            """, cancellationToken);
    }
}
