IF COL_LENGTH('MasgedSettings', 'ParentAppStoreUrl') IS NULL
BEGIN
    ALTER TABLE MasgedSettings ADD ParentAppStoreUrl NVARCHAR(500) NULL;
END

IF COL_LENGTH('MasgedSettings', 'ParentGooglePlayUrl') IS NULL
BEGIN
    ALTER TABLE MasgedSettings ADD ParentGooglePlayUrl NVARCHAR(500) NULL;
END

IF COL_LENGTH('MasgedSettings', 'TeacherAppStoreUrl') IS NULL
BEGIN
    ALTER TABLE MasgedSettings ADD TeacherAppStoreUrl NVARCHAR(500) NULL;
END

IF COL_LENGTH('MasgedSettings', 'TeacherGooglePlayUrl') IS NULL
BEGIN
    ALTER TABLE MasgedSettings ADD TeacherGooglePlayUrl NVARCHAR(500) NULL;
END
