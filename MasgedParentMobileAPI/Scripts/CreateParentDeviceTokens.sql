IF NOT EXISTS (
    SELECT 1 FROM sys.tables WHERE name = N'ParentDeviceTokens' AND schema_id = SCHEMA_ID(N'dbo')
)
BEGIN
    CREATE TABLE dbo.ParentDeviceTokens (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ParentPhone NVARCHAR(20) NOT NULL,
        FcmToken NVARCHAR(512) NOT NULL,
        Platform NVARCHAR(20) NOT NULL CONSTRAINT DF_ParentDeviceTokens_Platform DEFAULT N'',
        UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_ParentDeviceTokens_UpdatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE UNIQUE INDEX UX_ParentDeviceTokens_FcmToken ON dbo.ParentDeviceTokens (FcmToken);
    CREATE INDEX IX_ParentDeviceTokens_ParentPhone ON dbo.ParentDeviceTokens (ParentPhone);
END
