-- Run once on the shared MeetingsInfo table (teacher + parent API contexts).
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.MeetingsInfo') AND name = N'Status'
)
BEGIN
    ALTER TABLE dbo.MeetingsInfo ADD
        Status TINYINT NOT NULL CONSTRAINT DF_MeetingsInfo_Status DEFAULT 0,
        EndedAt DATETIME NULL,
        TeacherNotes NVARCHAR(MAX) NULL;
END
GO
