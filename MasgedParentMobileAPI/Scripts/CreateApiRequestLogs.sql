-- Run once against the Unified API database (e.g. NewMasgedTeacherAPIDB).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'ApiRequestLogs')
BEGIN
    CREATE TABLE dbo.ApiRequestLogs
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        RequestedAt DATETIME2(7) NOT NULL,
        Method NVARCHAR(10) NOT NULL,
        Path NVARCHAR(500) NOT NULL,
        QueryString NVARCHAR(2000) NULL,
        RequestHeaders NVARCHAR(MAX) NULL,
        RequestBody NVARCHAR(MAX) NULL,
        ResponseStatusCode INT NOT NULL,
        ResponseBody NVARCHAR(MAX) NULL,
        DurationMs INT NOT NULL,
        ClientIp NVARCHAR(64) NULL,
        UserId NVARCHAR(100) NULL,
        UserName NVARCHAR(200) NULL
    );

    CREATE NONCLUSTERED INDEX IX_ApiRequestLogs_RequestedAt
        ON dbo.ApiRequestLogs (RequestedAt DESC);
END
GO
