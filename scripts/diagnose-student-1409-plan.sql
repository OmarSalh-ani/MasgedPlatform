/*
  Diagnostic: student plan skip الملك → التحريم (student 1409)
  Run against the production / mosque SQL Server database.
  Paste results of sections 3, 5, and 6 back for root-cause confirmation.
*/
SET NOCOUNT ON;
DECLARE @StudentId INT = 1409;

------------------------------------------------------------
-- 0) Surah IDs (verify الملك / التحريم / الجمعة)
------------------------------------------------------------
SELECT Id, NameAr, SortOrder
FROM QuranSurah
WHERE NameAr LIKE N'%الملك%'
   OR NameAr LIKE N'%التحريم%'
   OR NameAr LIKE N'%الجمعة%'
ORDER BY Id;

------------------------------------------------------------
-- 1) Plans for this student
------------------------------------------------------------
SELECT
    p.Id AS PlanId,
    p.Name,
    p.PlanFromDate,
    p.PlanToDate,
    p.CreatedAt,
    p.IsArchived
FROM StudentPlan p
WHERE p.StudentId = @StudentId
ORDER BY p.PlanFromDate DESC, p.Id DESC;

------------------------------------------------------------
-- 2) All حفظ rows (ordered by day)
------------------------------------------------------------
SELECT
    m.Id,
    m.PlanId,
    m.PlanDate,
    m.PlanEndDate,
    m.SurahId,
    s.NameAr AS SurahName,
    s.SortOrder,
    m.FromAyahNumber,
    m.ToAyahNumber,
    m.Status,
    m.CreatedAt,
    m.MemorizationLevel
FROM StudentPlanMemorizing m
LEFT JOIN QuranSurah s ON s.Id = m.SurahId
WHERE m.StudentId = @StudentId
ORDER BY m.PlanId, m.PlanDate, m.Id;

------------------------------------------------------------
-- 3) Focus: الملك / التحريم / الجمعة only  << SHARE THIS
------------------------------------------------------------
SELECT
    m.Id,
    m.PlanId,
    m.PlanDate,
    s.NameAr AS SurahName,
    m.FromAyahNumber,
    m.ToAyahNumber,
    m.Status,
    m.CreatedAt
FROM StudentPlanMemorizing m
JOIN QuranSurah s ON s.Id = m.SurahId
WHERE m.StudentId = @StudentId
  AND (
        s.NameAr LIKE N'%الملك%'
     OR s.NameAr LIKE N'%التحريم%'
     OR s.NameAr LIKE N'%الجمعة%'
  )
ORDER BY m.PlanId, m.PlanDate, m.Id;

------------------------------------------------------------
-- 4) Gap detector within the same surah
------------------------------------------------------------
;WITH ordered AS (
    SELECT
        m.PlanId,
        m.SurahId,
        s.NameAr,
        m.Id,
        m.PlanDate,
        m.FromAyahNumber,
        m.ToAyahNumber,
        m.Status,
        LAG(m.ToAyahNumber) OVER (
            PARTITION BY m.PlanId, m.SurahId
            ORDER BY m.PlanDate, m.Id
        ) AS PrevTo,
        LAG(m.Id) OVER (
            PARTITION BY m.PlanId, m.SurahId
            ORDER BY m.PlanDate, m.Id
        ) AS PrevId
    FROM StudentPlanMemorizing m
    JOIN QuranSurah s ON s.Id = m.SurahId
    WHERE m.StudentId = @StudentId
)
SELECT
    PlanId,
    NameAr,
    PrevId,
    PrevTo,
    Id AS CurrentId,
    PlanDate,
    FromAyahNumber,
    ToAyahNumber,
    Status,
    CASE
        WHEN PrevTo IS NOT NULL AND FromAyahNumber > PrevTo + 1
            THEN N'GAP: skipped ayahs ' + CAST(PrevTo + 1 AS nvarchar(10))
               + N'-' + CAST(FromAyahNumber - 1 AS nvarchar(10))
        ELSE N'OK continuous'
    END AS GapCheck
FROM ordered
WHERE PrevTo IS NOT NULL
ORDER BY PlanId, SurahId, PlanDate, Id;

------------------------------------------------------------
-- 5) الملك truncated then jumped to another surah?  << SHARE THIS
------------------------------------------------------------
DECLARE @MulkId INT = (
    SELECT TOP 1 Id FROM QuranSurah WHERE NameAr LIKE N'%الملك%' ORDER BY Id
);
DECLARE @MulkMax INT = (
    SELECT ISNULL(MAX(aya_no), 30) FROM HolyQuran WHERE sura_no = @MulkId
);

;WITH rows AS (
    SELECT
        m.PlanId,
        m.Id,
        m.PlanDate,
        m.SurahId,
        s.NameAr,
        m.FromAyahNumber,
        m.ToAyahNumber,
        m.Status,
        LEAD(m.SurahId) OVER (PARTITION BY m.PlanId ORDER BY m.PlanDate, m.Id) AS NextSurahId,
        LEAD(m.FromAyahNumber) OVER (PARTITION BY m.PlanId ORDER BY m.PlanDate, m.Id) AS NextFrom,
        LEAD(m.ToAyahNumber) OVER (PARTITION BY m.PlanId ORDER BY m.PlanDate, m.Id) AS NextTo
    FROM StudentPlanMemorizing m
    JOIN QuranSurah s ON s.Id = m.SurahId
    WHERE m.StudentId = @StudentId
),
rowsNamed AS (
    SELECT
        r.*,
        ns.NameAr AS NextSurahName
    FROM rows r
    LEFT JOIN QuranSurah ns ON ns.Id = r.NextSurahId
)
SELECT
    PlanId,
    Id,
    PlanDate,
    SurahId,
    NameAr,
    FromAyahNumber,
    ToAyahNumber,
    Status,
    NextSurahId,
    NextSurahName,
    NextFrom,
    NextTo,
    @MulkMax AS MulkMaxAyah,
    CASE
        WHEN SurahId = @MulkId
         AND ToAyahNumber < @MulkMax
         AND NextSurahId IS NOT NULL
         AND NextSurahId <> @MulkId
         AND NOT EXISTS (
                SELECT 1 FROM rowsNamed r2
                WHERE r2.PlanId = rowsNamed.PlanId
                  AND r2.SurahId = @MulkId
                  AND r2.FromAyahNumber = rowsNamed.ToAyahNumber + 1
            )
            THEN N'SUSPECT: الملك truncated then jumped to ' + ISNULL(NextSurahName, N'?')
               + N' (no remainder row)'
        WHEN SurahId = @MulkId
         AND ToAyahNumber < @MulkMax
         AND EXISTS (
                SELECT 1 FROM rowsNamed r2
                WHERE r2.PlanId = rowsNamed.PlanId
                  AND r2.SurahId = @MulkId
                  AND r2.FromAyahNumber = rowsNamed.ToAyahNumber + 1
            )
            THEN N'OK: remainder of الملك exists'
        WHEN SurahId = @MulkId
         AND ToAyahNumber < @MulkMax
         AND NextSurahId IS NOT NULL
         AND NextSurahId <> @MulkId
            THEN N'SUSPECT: next row is ' + ISNULL(NextSurahName, N'?')
               + N' — check if remainder exists later'
        ELSE N'check manually'
    END AS Diagnosis
FROM rowsNamed
WHERE SurahId = @MulkId
   OR NameAr LIKE N'%التحريم%'
   OR NameAr LIKE N'%الجمعة%'
ORDER BY PlanId, PlanDate, Id;

------------------------------------------------------------
-- 6) Status logs joined to memorizing rows  << SHARE THIS
------------------------------------------------------------
SELECT
    l.Id AS LogId,
    l.LoggedAt,
    l.Status AS LogStatus,
    l.RowKey,
    l.TeacherId,
    m.Id AS MemId,
    m.PlanId,
    s.NameAr AS SurahName,
    m.FromAyahNumber,
    m.ToAyahNumber,
    m.PlanDate,
    m.Status AS CurrentRowStatus
FROM StudentPlanItemLog l
LEFT JOIN StudentPlanMemorizing m
  ON l.RowKey LIKE N'memorizing_%'
 AND m.Id = TRY_CAST(SUBSTRING(l.RowKey, LEN(N'memorizing_') + 1, 20) AS INT)
 AND m.StudentId = l.StudentId
LEFT JOIN QuranSurah s ON s.Id = m.SurahId
WHERE l.StudentId = @StudentId
ORDER BY l.LoggedAt DESC;

------------------------------------------------------------
-- 7) مراجعة rows
------------------------------------------------------------
SELECT
    r.Id,
    r.PlanId,
    r.PlanDate,
    s.NameAr,
    r.FromAyahNumber,
    r.ToAyahNumber,
    r.Status,
    r.CreatedAt
FROM StudentPlanRevise r
LEFT JOIN QuranSurah s ON s.Id = r.SurahId
WHERE r.StudentId = @StudentId
ORDER BY r.PlanId, r.PlanDate, r.Id;

------------------------------------------------------------
-- 8) Same-day collision: الملك remainder + التحريم on same PlanDate
------------------------------------------------------------
;WITH mem AS (
    SELECT
        m.PlanId,
        m.Id,
        m.PlanDate,
        m.SurahId,
        s.NameAr,
        m.FromAyahNumber,
        m.ToAyahNumber,
        m.Status
    FROM StudentPlanMemorizing m
    JOIN QuranSurah s ON s.Id = m.SurahId
    WHERE m.StudentId = @StudentId
      AND m.Status IN (N'منتظر التسميع', N'قيد الانتظار', N'اعادة تسميع', N'لم يتم الحفظ')
)
SELECT
    a.PlanId,
    a.PlanDate,
    a.Id AS RowA_Id,
    a.NameAr AS RowA_Surah,
    a.FromAyahNumber AS RowA_From,
    a.ToAyahNumber AS RowA_To,
    a.Status AS RowA_Status,
    b.Id AS RowB_Id,
    b.NameAr AS RowB_Surah,
    b.FromAyahNumber AS RowB_From,
    b.ToAyahNumber AS RowB_To,
    b.Status AS RowB_Status,
    N'Same PlanDate has multiple pending rows — UI may pick wrong "current"' AS Note
FROM mem a
JOIN mem b
  ON a.PlanId = b.PlanId
 AND a.PlanDate = b.PlanDate
 AND a.Id < b.Id
ORDER BY a.PlanId, a.PlanDate, a.Id;
