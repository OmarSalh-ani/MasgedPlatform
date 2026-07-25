/*
    PurgeDatabaseAndSeedAdmin.sql

    WARNING: Destructive script. Deletes all rows from every user table except Quran
    reference data (HolyQuran, QuranSurah, QuranAyah), reseeds the remaining identity
    columns, then creates a single admin teacher account.

    Target database: NewMasgedTeacherAPIDB

    Admin login (AdminPanel):
      Username: admin@admin.com   (login matches Teacher.Email)
      Password: admin

    Teacher record:
      Name:  admin
      Email: admin@admin.com
      UsersManage: 1 (full admin panel access)
*/

USE [NewMasgedTeacherAPIDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @sql NVARCHAR(MAX);

DECLARE @PreservedTables TABLE (TableName SYSNAME PRIMARY KEY);
INSERT INTO @PreservedTables (TableName)
VALUES (N'HolyQuran'), (N'QuranSurah'), (N'QuranAyah');

PRINT N'Disabling foreign key constraints...';

SET @sql = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N' NOCHECK CONSTRAINT ALL;' + CHAR(13)
FROM sys.tables AS t
WHERE t.is_ms_shipped = 0
  AND t.is_external = 0;

EXEC sys.sp_executesql @sql;

PRINT N'Deleting table data (preserving HolyQuran, QuranSurah, QuranAyah)...';

SET @sql = N'';
SELECT @sql += N'DELETE FROM ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N';' + CHAR(13)
FROM sys.tables AS t
WHERE t.is_ms_shipped = 0
  AND t.is_external = 0
  AND NOT EXISTS (
      SELECT 1
      FROM @PreservedTables AS p
      WHERE p.TableName = t.name
  );

EXEC sys.sp_executesql @sql;

PRINT N'Reseeding identity columns (excluding preserved Quran tables)...';

DECLARE @reseedSql NVARCHAR(MAX);
DECLARE @schemaName SYSNAME;
DECLARE @tableName SYSNAME;

DECLARE identity_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT SCHEMA_NAME(t.schema_id), t.name
FROM sys.tables AS t
INNER JOIN sys.identity_columns AS ic ON ic.object_id = t.object_id
WHERE t.is_ms_shipped = 0
  AND t.is_external = 0
  AND NOT EXISTS (
      SELECT 1
      FROM @PreservedTables AS p
      WHERE p.TableName = t.name
  );

OPEN identity_cursor;
FETCH NEXT FROM identity_cursor INTO @schemaName, @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @reseedSql = N'DBCC CHECKIDENT (' + QUOTENAME(@schemaName + N'.' + @tableName, '''') + N', RESEED, 0) WITH NO_INFOMSGS;';
    EXEC sys.sp_executesql @reseedSql;

    FETCH NEXT FROM identity_cursor INTO @schemaName, @tableName;
END

CLOSE identity_cursor;
DEALLOCATE identity_cursor;

PRINT N'Re-enabling foreign key constraints...';

SET @sql = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N' WITH CHECK CHECK CONSTRAINT ALL;' + CHAR(13)
FROM sys.tables AS t
WHERE t.is_ms_shipped = 0
  AND t.is_external = 0;

EXEC sys.sp_executesql @sql;

PRINT N'Creating admin teacher...';

INSERT INTO dbo.Teacher
(
    Name,
    Email,
    Password,
    UsersManage,
    IsGirlTeacher,
    IsViewOnly,
    Mobile,
    Image,
    BaseSalary,
    AttendanceFingerprintHash
)
VALUES
(
    N'admin',
    N'admin@admin.com',
    N'admin',
    1,
    0,
    0,
    NULL,
    NULL,
    NULL,
    NULL
);

DECLARE @AdminTeacherId INT = CAST(SCOPE_IDENTITY() AS INT);

IF OBJECT_ID(N'dbo.MasgedSettings', N'U') IS NOT NULL
BEGIN
    PRINT N'Seeding default MasgedSettings row...';

    INSERT INTO dbo.MasgedSettings (MasgedName, LogoFileName, UpdatedAt)
    VALUES (N'Masged', NULL, GETDATE());
END

COMMIT TRANSACTION;

PRINT N'Done. Admin teacher Id = ' + CAST(@AdminTeacherId AS NVARCHAR(20));
GO
