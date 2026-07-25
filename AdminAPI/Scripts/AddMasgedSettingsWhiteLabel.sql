IF COL_LENGTH('MasgedSettings', 'PrimaryColor') IS NULL
    ALTER TABLE MasgedSettings ADD PrimaryColor NVARCHAR(20) NULL;

IF COL_LENGTH('MasgedSettings', 'Domain') IS NULL
    ALTER TABLE MasgedSettings ADD Domain NVARCHAR(200) NULL;

IF COL_LENGTH('MasgedSettings', 'SetupCompleted') IS NULL
BEGIN
    ALTER TABLE MasgedSettings ADD SetupCompleted BIT NOT NULL CONSTRAINT DF_MasgedSettings_SetupCompleted DEFAULT (0);
    UPDATE MasgedSettings SET SetupCompleted = 1 WHERE LEN(LTRIM(RTRIM(ISNULL(MasgedName, N'')))) > 0;
END
