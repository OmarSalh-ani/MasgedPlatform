-- Run once against the Unified API database (e.g. NewMasgedTeacherAPIDB).
-- Same table as MasgedParentMobileAPI/Scripts/CreatePushDeliveryLogs.sql — run only once.
IF NOT EXISTS (
    SELECT 1 FROM sys.tables WHERE name = N'PushDeliveryLogs' AND schema_id = SCHEMA_ID(N'dbo')
)
BEGIN
    CREATE TABLE dbo.PushDeliveryLogs (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_PushDeliveryLogs_CreatedAt DEFAULT SYSUTCDATETIME(),
        Source NVARCHAR(40) NOT NULL,
        Context NVARCHAR(200) NOT NULL,
        AudienceKind NVARCHAR(20) NOT NULL,
        Platform NVARCHAR(20) NOT NULL CONSTRAINT DF_PushDeliveryLogs_Platform DEFAULT N'',
        OwnerKey NVARCHAR(64) NULL,
        FcmToken NVARCHAR(512) NULL,
        Success BIT NOT NULL,
        ErrorCode NVARCHAR(100) NULL,
        ErrorDetail NVARCHAR(2000) NULL,
        MessageId NVARCHAR(200) NULL,
        Title NVARCHAR(200) NULL,
        BodyPreview NVARCHAR(300) NULL
    );

    CREATE NONCLUSTERED INDEX IX_PushDeliveryLogs_CreatedAt
        ON dbo.PushDeliveryLogs (CreatedAt DESC);

    CREATE NONCLUSTERED INDEX IX_PushDeliveryLogs_Success_CreatedAt
        ON dbo.PushDeliveryLogs (Success, CreatedAt DESC);

    CREATE NONCLUSTERED INDEX IX_PushDeliveryLogs_Platform_CreatedAt
        ON dbo.PushDeliveryLogs (Platform, CreatedAt DESC);
END
GO
