-- StudentPlanMemorizing / StudentPlanRevise have no index on StudentId or PlanId,
-- so every parent/teacher screen that reads a student's latest plan row scans the
-- whole table (parent profile, quran-assignment, teacher home plan-level fallback,
-- plan progress). Run on the production DB (idempotent).

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_StudentPlanMemorizing_StudentId_PlanDate'
      AND object_id = OBJECT_ID(N'dbo.StudentPlanMemorizing')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_StudentPlanMemorizing_StudentId_PlanDate
    ON dbo.StudentPlanMemorizing (StudentId, PlanDate DESC)
    INCLUDE (MemorizationLevel);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_StudentPlanRevise_StudentId_PlanDate'
      AND object_id = OBJECT_ID(N'dbo.StudentPlanRevise')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_StudentPlanRevise_StudentId_PlanDate
    ON dbo.StudentPlanRevise (StudentId, PlanDate DESC)
    INCLUDE (MemorizationLevel);
END
GO

-- Plan-scoped reads (progress, row lists, next-revise lookup) filter by PlanId only.

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_StudentPlanMemorizing_PlanId_StudentId'
      AND object_id = OBJECT_ID(N'dbo.StudentPlanMemorizing')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_StudentPlanMemorizing_PlanId_StudentId
    ON dbo.StudentPlanMemorizing (PlanId, StudentId)
    INCLUDE (Status, PlanDate, MemorizationLevel);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_StudentPlanRevise_PlanId_StudentId'
      AND object_id = OBJECT_ID(N'dbo.StudentPlanRevise')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_StudentPlanRevise_PlanId_StudentId
    ON dbo.StudentPlanRevise (PlanId, StudentId)
    INCLUDE (Status, PlanDate, MemorizationLevel);
END
GO
