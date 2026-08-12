IF OBJECT_ID(N'dbo.MasgedSettings', N'U') IS NULL
BEGIN
    RAISERROR(N'MasgedSettings table not found.', 16, 1);
    RETURN;
END
GO

IF COL_LENGTH('MasgedSettings', 'PrimaryColor') IS NULL
    ALTER TABLE MasgedSettings ADD PrimaryColor NVARCHAR(20) NULL;
GO
