IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'MasgedSettings')
BEGIN
    CREATE TABLE MasgedSettings (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MasgedName NVARCHAR(200) NOT NULL,
        LogoFileName NVARCHAR(500) NULL,
        UpdatedAt DATETIME NULL
    );

    INSERT INTO MasgedSettings (MasgedName, LogoFileName, UpdatedAt)
    VALUES (N'مسجد الشيخ مبارك عبدالله المبارك الصباح', NULL, GETDATE());
END
