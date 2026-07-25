/*
    CopyShaikhMubaraToNewMasgedTeacherAPI.sql

    Copies row data from ShaikhMubaraDB (source) into NewMasgedTeacherAPIDB (target)
    for every user table that exists in BOTH databases with the same schema and name.

    Behaviour:
      1. Reports tables present in only one database (skipped).
      2. Reports column mismatches per table (copies intersection of column names only).
      3. Disables FK constraints on the target during copy.
      4. Inserts source rows into the target without deleting existing target data.
         Skips rows that already exist (matched by primary key, or identity column,
         or all shared columns when no key is defined).
      5. Copies in batches of @BatchSize rows (default 200) per table.
      6. SET IDENTITY_INSERT ON before insert when the target table has an identity column,
         then SET IDENTITY_INSERT OFF after each table.
      7. Reseeds identity seeds on the target to MAX(id) after copy.
      8. Re-enables FK constraints on the target.

    Existing rows in the target are left unchanged. Rows with matching keys are skipped.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @SourceDb SYSNAME = N'ShaikhMubaraDB';
DECLARE @TargetDb SYSNAME = N'NewMasgedTeacherAPIDB';
DECLARE @BatchSize INT = 200;

IF DB_ID(@SourceDb) IS NULL
BEGIN
    RAISERROR(N'Source database [%s] was not found on this server.', 16, 1, @SourceDb);
    RETURN;
END;

IF DB_ID(@TargetDb) IS NULL
BEGIN
    RAISERROR(N'Target database [%s] was not found on this server.', 16, 1, @TargetDb);
    RETURN;
END;

PRINT N'=== Database copy: ' + @SourceDb + N' -> ' + @TargetDb + N' ===';
PRINT N'';

/* -------------------------------------------------------------------------
   Discovery: tables only in source or only in target
   ------------------------------------------------------------------------- */
PRINT N'--- Tables only in SOURCE (not copied) ---';

SELECT
    s.TABLE_SCHEMA,
    s.TABLE_NAME
FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.TABLES AS s
WHERE s.TABLE_TYPE = N'BASE TABLE'
  AND NOT EXISTS (
      SELECT 1
      FROM [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.TABLES AS t
      WHERE t.TABLE_SCHEMA = s.TABLE_SCHEMA
        AND t.TABLE_NAME = s.TABLE_NAME
        AND t.TABLE_TYPE = N'BASE TABLE'
  )
ORDER BY s.TABLE_SCHEMA, s.TABLE_NAME;

PRINT N'--- Tables only in TARGET (left unchanged) ---';

SELECT
    t.TABLE_SCHEMA,
    t.TABLE_NAME
FROM [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.TABLES AS t
WHERE t.TABLE_TYPE = N'BASE TABLE'
  AND NOT EXISTS (
      SELECT 1
      FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.TABLES AS s
      WHERE s.TABLE_SCHEMA = t.TABLE_SCHEMA
        AND s.TABLE_NAME = t.TABLE_NAME
        AND s.TABLE_TYPE = N'BASE TABLE'
  )
ORDER BY t.TABLE_SCHEMA, t.TABLE_NAME;

PRINT N'--- Tables in BOTH databases (eligible for copy) ---';

DECLARE @MatchedTables TABLE
(
    TableSchema SYSNAME NOT NULL,
    TableName   SYSNAME NOT NULL,
    PRIMARY KEY (TableSchema, TableName)
);

INSERT INTO @MatchedTables (TableSchema, TableName)
SELECT
    s.TABLE_SCHEMA,
    s.TABLE_NAME
FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.TABLES AS s
INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.TABLES AS t
    ON t.TABLE_SCHEMA = s.TABLE_SCHEMA
   AND t.TABLE_NAME = s.TABLE_NAME
WHERE s.TABLE_TYPE = N'BASE TABLE'
  AND t.TABLE_TYPE = N'BASE TABLE'
ORDER BY s.TABLE_SCHEMA, s.TABLE_NAME;

SELECT TableSchema, TableName
FROM @MatchedTables
ORDER BY TableSchema, TableName;

IF NOT EXISTS (SELECT 1 FROM @MatchedTables)
BEGIN
    RAISERROR(N'No matching table names were found between the two databases.', 16, 1);
    RETURN;
END;

BEGIN TRANSACTION;

DECLARE @schemaName SYSNAME;
DECLARE @tableName SYSNAME;
DECLARE @qualifiedTarget NVARCHAR(517);
DECLARE @qualifiedSource NVARCHAR(517);
DECLARE @targetTableTwoPart NVARCHAR(260);
DECLARE @targetDbExec NVARCHAR(300);
DECLARE @columnList NVARCHAR(MAX);
DECLARE @selectColumnList NVARCHAR(MAX);
DECLARE @notExistsPredicate NVARCHAR(MAX);
DECLARE @orderByClause NVARCHAR(MAX);
DECLARE @sourceOnlyColumns NVARCHAR(MAX);
DECLARE @targetOnlyColumns NVARCHAR(MAX);
DECLARE @identityColumnName SYSNAME;
DECLARE @needsIdentityInsert BIT;
DECLARE @insertedCount INT;
DECLARE @tableInsertedTotal INT;
DECLARE @batchNumber INT;
DECLARE @rowCount BIGINT;
DECLARE @sql NVARCHAR(MAX);
DECLARE @reseedSql NVARCHAR(MAX);
DECLARE @maxIdentitySql NVARCHAR(MAX);
DECLARE @maxIdentityValue SQL_VARIANT;
DECLARE @cmd NVARCHAR(MAX);

SET @targetDbExec = QUOTENAME(@TargetDb) + N'.sys.sp_executesql';

PRINT N'';
PRINT N'Disabling foreign key constraints on target...';

SET @sql = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(@TargetDb) + N'.' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N' NOCHECK CONSTRAINT ALL;' + CHAR(13)
FROM [NewMasgedTeacherAPIDB].sys.tables AS t
WHERE t.is_ms_shipped = 0
  AND t.is_external = 0;

EXEC sys.sp_executesql @sql;

DECLARE table_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT TableSchema, TableName
FROM @MatchedTables
ORDER BY TableSchema, TableName;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @schemaName, @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @identityColumnName = NULL;
    SET @sourceOnlyColumns = NULL;
    SET @targetOnlyColumns = NULL;
    SET @notExistsPredicate = NULL;
    SET @orderByClause = NULL;

    SET @qualifiedSource = QUOTENAME(@SourceDb) + N'.' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@tableName);
    SET @qualifiedTarget = QUOTENAME(@TargetDb) + N'.' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@tableName);
    SET @targetTableTwoPart = QUOTENAME(@schemaName) + N'.' + QUOTENAME(@tableName);

    SELECT @columnList = STRING_AGG(QUOTENAME(s.COLUMN_NAME), N', ') WITHIN GROUP (ORDER BY s.ORDINAL_POSITION)
    FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS s
    INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS t
        ON t.TABLE_SCHEMA = s.TABLE_SCHEMA
       AND t.TABLE_NAME = s.TABLE_NAME
       AND t.COLUMN_NAME = s.COLUMN_NAME
    WHERE s.TABLE_SCHEMA = @schemaName
      AND s.TABLE_NAME = @tableName;

    SELECT @selectColumnList = STRING_AGG(N's.' + QUOTENAME(s.COLUMN_NAME), N', ') WITHIN GROUP (ORDER BY s.ORDINAL_POSITION)
    FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS s
    INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS t
        ON t.TABLE_SCHEMA = s.TABLE_SCHEMA
       AND t.TABLE_NAME = s.TABLE_NAME
       AND t.COLUMN_NAME = s.COLUMN_NAME
    WHERE s.TABLE_SCHEMA = @schemaName
      AND s.TABLE_NAME = @tableName;

    SELECT @sourceOnlyColumns = STRING_AGG(QUOTENAME(s.COLUMN_NAME), N', ')
    FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS s
    WHERE s.TABLE_SCHEMA = @schemaName
      AND s.TABLE_NAME = @tableName
      AND NOT EXISTS (
          SELECT 1
          FROM [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS t
          WHERE t.TABLE_SCHEMA = s.TABLE_SCHEMA
            AND t.TABLE_NAME = s.TABLE_NAME
            AND t.COLUMN_NAME = s.COLUMN_NAME
      );

    SELECT @targetOnlyColumns = STRING_AGG(QUOTENAME(t.COLUMN_NAME), N', ')
    FROM [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS t
    WHERE t.TABLE_SCHEMA = @schemaName
      AND t.TABLE_NAME = @tableName
      AND NOT EXISTS (
          SELECT 1
          FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS s
          WHERE s.TABLE_SCHEMA = t.TABLE_SCHEMA
            AND s.TABLE_NAME = t.TABLE_NAME
            AND s.COLUMN_NAME = t.COLUMN_NAME
      );

    IF @columnList IS NULL OR LEN(@columnList) = 0
    BEGIN
        PRINT N'[SKIP] ' + @schemaName + N'.' + @tableName + N' — no shared columns.';
        GOTO NextTable;
    END;

    IF @sourceOnlyColumns IS NOT NULL OR @targetOnlyColumns IS NOT NULL
    BEGIN
        PRINT N'[WARN] ' + @schemaName + N'.' + @tableName
            + CASE WHEN @sourceOnlyColumns IS NOT NULL THEN N' | source-only: ' + @sourceOnlyColumns ELSE N'' END
            + CASE WHEN @targetOnlyColumns IS NOT NULL THEN N' | target-only: ' + @targetOnlyColumns ELSE N'' END;
    END;

    SELECT TOP (1) @identityColumnName = ic.name
    FROM [NewMasgedTeacherAPIDB].sys.identity_columns AS ic
    WHERE ic.object_id = OBJECT_ID(@qualifiedTarget);

    SELECT @notExistsPredicate = STRING_AGG(N't.' + QUOTENAME(c.name) + N' = s.' + QUOTENAME(c.name), N' AND ')
        WITHIN GROUP (ORDER BY ic.key_ordinal)
    FROM [NewMasgedTeacherAPIDB].sys.indexes AS i
    INNER JOIN [NewMasgedTeacherAPIDB].sys.index_columns AS ic
        ON ic.object_id = i.object_id
       AND ic.index_id = i.index_id
    INNER JOIN [NewMasgedTeacherAPIDB].sys.columns AS c
        ON c.object_id = ic.object_id
       AND c.column_id = ic.column_id
    INNER JOIN [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS sc
        ON sc.TABLE_SCHEMA = @schemaName
       AND sc.TABLE_NAME = @tableName
       AND sc.COLUMN_NAME = c.name
    INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS tc
        ON tc.TABLE_SCHEMA = @schemaName
       AND tc.TABLE_NAME = @tableName
       AND tc.COLUMN_NAME = c.name
    WHERE i.object_id = OBJECT_ID(@qualifiedTarget)
      AND i.is_primary_key = 1;

    IF @notExistsPredicate IS NULL AND @identityColumnName IS NOT NULL
        AND EXISTS (
            SELECT 1
            FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS s
            INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS t
                ON t.TABLE_SCHEMA = s.TABLE_SCHEMA
               AND t.TABLE_NAME = s.TABLE_NAME
               AND t.COLUMN_NAME = s.COLUMN_NAME
            WHERE s.TABLE_SCHEMA = @schemaName
              AND s.TABLE_NAME = @tableName
              AND s.COLUMN_NAME = @identityColumnName
        )
    BEGIN
        SET @notExistsPredicate = N't.' + QUOTENAME(@identityColumnName) + N' = s.' + QUOTENAME(@identityColumnName);
    END;

    IF @notExistsPredicate IS NULL
    BEGIN
        SELECT @notExistsPredicate = STRING_AGG(
            N'(t.' + QUOTENAME(s.COLUMN_NAME) + N' = s.' + QUOTENAME(s.COLUMN_NAME)
            + N' OR (t.' + QUOTENAME(s.COLUMN_NAME) + N' IS NULL AND s.' + QUOTENAME(s.COLUMN_NAME) + N' IS NULL))',
            N' AND ')
            WITHIN GROUP (ORDER BY s.ORDINAL_POSITION)
        FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS s
        INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS t
            ON t.TABLE_SCHEMA = s.TABLE_SCHEMA
           AND t.TABLE_NAME = s.TABLE_NAME
           AND t.COLUMN_NAME = s.COLUMN_NAME
        WHERE s.TABLE_SCHEMA = @schemaName
          AND s.TABLE_NAME = @tableName;
    END;

    SELECT @orderByClause = STRING_AGG(N's.' + QUOTENAME(c.name), N', ')
        WITHIN GROUP (ORDER BY ic.key_ordinal)
    FROM [NewMasgedTeacherAPIDB].sys.indexes AS i
    INNER JOIN [NewMasgedTeacherAPIDB].sys.index_columns AS ic
        ON ic.object_id = i.object_id
       AND ic.index_id = i.index_id
    INNER JOIN [NewMasgedTeacherAPIDB].sys.columns AS c
        ON c.object_id = ic.object_id
       AND c.column_id = ic.column_id
    INNER JOIN [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS sc
        ON sc.TABLE_SCHEMA = @schemaName
       AND sc.TABLE_NAME = @tableName
       AND sc.COLUMN_NAME = c.name
    INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS tc
        ON tc.TABLE_SCHEMA = @schemaName
       AND tc.TABLE_NAME = @tableName
       AND tc.COLUMN_NAME = c.name
    WHERE i.object_id = OBJECT_ID(@qualifiedTarget)
      AND i.is_primary_key = 1;

    IF @orderByClause IS NULL AND @identityColumnName IS NOT NULL
        AND EXISTS (
            SELECT 1
            FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS s
            INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS t
                ON t.TABLE_SCHEMA = s.TABLE_SCHEMA
               AND t.TABLE_NAME = s.TABLE_NAME
               AND t.COLUMN_NAME = s.COLUMN_NAME
            WHERE s.TABLE_SCHEMA = @schemaName
              AND s.TABLE_NAME = @tableName
              AND s.COLUMN_NAME = @identityColumnName
        )
    BEGIN
        SET @orderByClause = N's.' + QUOTENAME(@identityColumnName);
    END;

    IF @orderByClause IS NULL
    BEGIN
        SELECT TOP (1) @orderByClause = N's.' + QUOTENAME(s.COLUMN_NAME)
        FROM [ShaikhMubaraDB].INFORMATION_SCHEMA.COLUMNS AS s
        INNER JOIN [NewMasgedTeacherAPIDB].INFORMATION_SCHEMA.COLUMNS AS t
            ON t.TABLE_SCHEMA = s.TABLE_SCHEMA
           AND t.TABLE_NAME = s.TABLE_NAME
           AND t.COLUMN_NAME = s.COLUMN_NAME
        WHERE s.TABLE_SCHEMA = @schemaName
          AND s.TABLE_NAME = @tableName
        ORDER BY s.ORDINAL_POSITION;
    END;

    SET @needsIdentityInsert = CASE
        WHEN @identityColumnName IS NOT NULL
         AND CHARINDEX(QUOTENAME(@identityColumnName), @columnList) > 0
        THEN 1
        ELSE 0
    END;

    BEGIN TRY
        SET @tableInsertedTotal = 0;
        SET @batchNumber = 0;

        WHILE 1 = 1
        BEGIN
            SET @insertedCount = 0;

            IF @needsIdentityInsert = 1
            BEGIN
                SET @sql = N'
SET IDENTITY_INSERT ' + @targetTableTwoPart + N' ON;

INSERT INTO ' + @targetTableTwoPart + N' (' + @columnList + N')
SELECT TOP (@BatchSize) ' + @selectColumnList + N'
FROM ' + @qualifiedSource + N' AS s
WHERE NOT EXISTS (
    SELECT 1
    FROM ' + @targetTableTwoPart + N' AS t
    WHERE ' + @notExistsPredicate + N'
)
ORDER BY ' + @orderByClause + N';

SELECT @insertedCount = @@ROWCOUNT;';

                SET @cmd = @targetDbExec + N' @sql, N''@BatchSize INT, @insertedCount INT OUTPUT'', @BatchSize, @insertedCount OUTPUT';
                EXEC sys.sp_executesql
                    @cmd,
                    N'@sql NVARCHAR(MAX), @BatchSize INT, @insertedCount INT OUTPUT',
                    @sql,
                    @BatchSize,
                    @insertedCount OUTPUT;
            END
            ELSE
            BEGIN
                SET @sql = N'
INSERT INTO ' + @qualifiedTarget + N' (' + @columnList + N')
SELECT TOP (@BatchSize) ' + @selectColumnList + N'
FROM ' + @qualifiedSource + N' AS s
WHERE NOT EXISTS (
    SELECT 1
    FROM ' + @qualifiedTarget + N' AS t
    WHERE ' + @notExistsPredicate + N'
)
ORDER BY ' + @orderByClause + N';
SELECT @insertedCount = @@ROWCOUNT;';

                EXEC sys.sp_executesql
                    @sql,
                    N'@BatchSize INT, @insertedCount INT OUTPUT',
                    @BatchSize,
                    @insertedCount OUTPUT;
            END;

            SET @tableInsertedTotal += @insertedCount;

            IF @insertedCount = 0
                BREAK;

            SET @batchNumber += 1;

            PRINT N'       batch ' + CAST(@batchNumber AS NVARCHAR(10))
                + N': inserted ' + CAST(@insertedCount AS NVARCHAR(10)) + N' row(s).';
        END;

        SET @insertedCount = @tableInsertedTotal;

        IF @needsIdentityInsert = 1
        BEGIN
            SET @sql = N'SET IDENTITY_INSERT ' + @targetTableTwoPart + N' OFF;';
            SET @cmd = @targetDbExec + N' @sql';
            EXEC sys.sp_executesql @cmd, N'@sql NVARCHAR(MAX)', @sql;

            SET @maxIdentitySql = N'SELECT @maxIdentityValue = MAX(' + QUOTENAME(@identityColumnName) + N') FROM ' + @targetTableTwoPart + N';';
            SET @cmd = @targetDbExec + N' @maxIdentitySql, N''@maxIdentityValue SQL_VARIANT OUTPUT'', @maxIdentityValue OUTPUT';

            EXEC sys.sp_executesql
                @cmd,
                N'@maxIdentitySql NVARCHAR(MAX), @maxIdentityValue SQL_VARIANT OUTPUT',
                @maxIdentitySql,
                @maxIdentityValue OUTPUT;

            IF @maxIdentityValue IS NOT NULL
            BEGIN
                SET @reseedSql = N'DBCC CHECKIDENT (' + QUOTENAME(@schemaName + N'.' + @tableName, '''') + N', RESEED, ' + CONVERT(NVARCHAR(30), @maxIdentityValue) + N') WITH NO_INFOMSGS;';
                SET @cmd = @targetDbExec + N' @reseedSql';
                EXEC sys.sp_executesql @cmd, N'@reseedSql NVARCHAR(MAX)', @reseedSql;
            END;
        END;

        SET @sql = N'SELECT @rowCount = COUNT_BIG(*) FROM ' + @qualifiedTarget + N';';
        EXEC sys.sp_executesql @sql, N'@rowCount BIGINT OUTPUT', @rowCount OUTPUT;

        PRINT N'[OK]   ' + @schemaName + N'.' + @tableName
            + N' — inserted ' + CAST(@insertedCount AS NVARCHAR(30))
            + N' in ' + CAST(@batchNumber AS NVARCHAR(10)) + N' batch(es)'
            + N', total ' + CAST(@rowCount AS NVARCHAR(30)) + N' row(s) in target.';
    END TRY
    BEGIN CATCH
        IF @needsIdentityInsert = 1
        BEGIN
            SET @sql = N'SET IDENTITY_INSERT ' + @targetTableTwoPart + N' OFF;';
            SET @cmd = @targetDbExec + N' @sql';
            EXEC sys.sp_executesql @cmd, N'@sql NVARCHAR(MAX)', @sql;
        END;

        PRINT N'[FAIL] ' + @schemaName + N'.' + @tableName + N' — ' + ERROR_MESSAGE();
        THROW;
    END CATCH;

NextTable:
    FETCH NEXT FROM table_cursor INTO @schemaName, @tableName;
END;

CLOSE table_cursor;
DEALLOCATE table_cursor;

PRINT N'';
PRINT N'Re-enabling foreign key constraints on target...';

SET @sql = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(@TargetDb) + N'.' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N' WITH CHECK CHECK CONSTRAINT ALL;' + CHAR(13)
FROM [NewMasgedTeacherAPIDB].sys.tables AS t
WHERE t.is_ms_shipped = 0
  AND t.is_external = 0;

EXEC sys.sp_executesql @sql;

COMMIT TRANSACTION;

PRINT N'';
PRINT N'Done.';
GO
