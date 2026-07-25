IF NOT EXISTS (
    SELECT 1 FROM sys.tables WHERE name = N'TeacherDeviceTokens' AND schema_id = SCHEMA_ID(N'dbo')
)
BEGIN
    CREATE TABLE dbo.TeacherDeviceTokens (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TeacherId INT NOT NULL,
        FcmToken NVARCHAR(512) NOT NULL,
        Platform NVARCHAR(20) NOT NULL CONSTRAINT DF_TeacherDeviceTokens_Platform DEFAULT N'',
        UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_TeacherDeviceTokens_UpdatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE UNIQUE INDEX UX_TeacherDeviceTokens_FcmToken ON dbo.TeacherDeviceTokens (FcmToken);
    CREATE INDEX IX_TeacherDeviceTokens_TeacherId ON dbo.TeacherDeviceTokens (TeacherId);
END
