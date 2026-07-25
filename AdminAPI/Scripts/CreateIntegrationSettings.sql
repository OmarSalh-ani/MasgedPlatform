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
