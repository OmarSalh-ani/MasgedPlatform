IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'Mosque') AND name = 'ImageUrl'
)
BEGIN
    ALTER TABLE Mosque ADD ImageUrl NVARCHAR(500) NULL;
END
