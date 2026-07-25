IF NOT EXISTS (
    SELECT 1 FROM sys.tables WHERE name = N'StudentCircleEnrollment' AND schema_id = SCHEMA_ID(N'dbo')
)
BEGIN
    CREATE TABLE dbo.StudentCircleEnrollment (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        CircleId INT NOT NULL,
        StartDate DATETIME2 NOT NULL CONSTRAINT DF_StudentCircleEnrollment_StartDate DEFAULT SYSUTCDATETIME(),
        EndDate DATETIME2 NULL,
        AssignedByTeacherId INT NULL,
        CONSTRAINT FK_StudentCircleEnrollment_RegisterForm
            FOREIGN KEY (StudentId) REFERENCES dbo.RegisterForm (Id),
        CONSTRAINT FK_StudentCircleEnrollment_QuranCircle
            FOREIGN KEY (CircleId) REFERENCES dbo.QuranCircle (Id),
        CONSTRAINT FK_StudentCircleEnrollment_Teacher
            FOREIGN KEY (AssignedByTeacherId) REFERENCES dbo.Teacher (Id)
    );

    CREATE INDEX IX_StudentCircleEnrollment_StudentId ON dbo.StudentCircleEnrollment (StudentId);
    CREATE INDEX IX_StudentCircleEnrollment_CircleId ON dbo.StudentCircleEnrollment (CircleId);
    CREATE INDEX IX_StudentCircleEnrollment_StudentId_EndDate ON dbo.StudentCircleEnrollment (StudentId, EndDate);
END
GO

-- Active enrollments for students currently assigned to a circle
INSERT INTO dbo.StudentCircleEnrollment (StudentId, CircleId, StartDate, EndDate, AssignedByTeacherId)
SELECT rf.Id, rf.QuranCircleId, COALESCE(rf.CreatedAt, SYSUTCDATETIME()), NULL, NULL
FROM dbo.RegisterForm rf
WHERE rf.QuranCircleId IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM dbo.StudentCircleEnrollment e
      WHERE e.StudentId = rf.Id AND e.CircleId = rf.QuranCircleId AND e.EndDate IS NULL
  );
GO

-- Closed enrollments from historical circle-scoped activity (past circles different from current)
;WITH HistoricalCircles AS (
    SELECT StudentId, CircleId FROM dbo.TestHead WHERE CircleId IS NOT NULL
    UNION
    SELECT StudentId, CircleId FROM dbo.StudentMemorizingCard WHERE CircleId IS NOT NULL
    UNION
    SELECT StudentId, CircleId FROM dbo.CircleAttendance WHERE CircleId IS NOT NULL
    UNION
    SELECT StudentId, CircleId FROM dbo.CircleDeparture WHERE CircleId IS NOT NULL
    UNION
    SELECT StudentId, CircleId FROM dbo.StudentTest WHERE CircleId IS NOT NULL
),
PastOnly AS (
    SELECT hc.StudentId, hc.CircleId
    FROM HistoricalCircles hc
    INNER JOIN dbo.RegisterForm rf ON rf.Id = hc.StudentId
    WHERE rf.QuranCircleId IS NULL OR hc.CircleId <> rf.QuranCircleId
)
INSERT INTO dbo.StudentCircleEnrollment (StudentId, CircleId, StartDate, EndDate, AssignedByTeacherId)
SELECT po.StudentId,
       po.CircleId,
       COALESCE((
           SELECT MIN(d) FROM (
               SELECT MIN(th.CreatedAt) AS d FROM dbo.TestHead th
               WHERE th.StudentId = po.StudentId AND th.CircleId = po.CircleId
               UNION ALL
               SELECT MIN(ca.AttendanceDateTime) FROM dbo.CircleAttendance ca
               WHERE ca.StudentId = po.StudentId AND ca.CircleId = po.CircleId
               UNION ALL
               SELECT MIN(sm.CreatedAt) FROM dbo.StudentMemorizingCard sm
               WHERE sm.StudentId = po.StudentId AND sm.CircleId = po.CircleId
           ) x
       ), SYSUTCDATETIME()),
       SYSUTCDATETIME(),
       NULL
FROM PastOnly po
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.StudentCircleEnrollment e
    WHERE e.StudentId = po.StudentId AND e.CircleId = po.CircleId
);
GO
