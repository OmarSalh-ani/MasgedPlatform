/*
  Fix ALL students affected by: partial تم الحفظ remainder scheduled on the
  NEXT day while other surahs stayed on the original PlanDate — so the app
  showed التحريم (etc.) before finishing the current surah.

  Pattern detected:
    Row A: تم الحفظ, Surah S, ToAyah = T, PlanDate = D
    Row B: منتظر التسميع, same Surah S, FromAyah = T+1, PlanDate > D  (remainder)
    Row C+: other Surah(s), pending, PlanDate = D                     (competitors)

  Fix:
    1) Move remainder B → PlanDate D (same day as the truncated pass)
    2) Move competitors C → PlanDate that B had (usually next work day)

  Also applies the same logic to StudentPlanRevise.

  HOW TO RUN:
    1) Run Part 1 (preview) and review counts/rows
    2) Run Part 2 inside a transaction
    3) Re-run Part 1 — should return 0 affected pairs
    4) COMMIT or ROLLBACK
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

------------------------------------------------------------
-- Part 1: PREVIEW affected memorizing pairs
------------------------------------------------------------
;WITH passRows AS (
    SELECT
        m.Id,
        m.StudentId,
        m.PlanId,
        m.SurahId,
        m.FromAyahNumber,
        m.ToAyahNumber,
        CAST(m.PlanDate AS date) AS PlanDate,
        m.Status
    FROM StudentPlanMemorizing m
    WHERE m.Status IN (N'تم الحفظ', N'تم', N'قيد الانتظار في التثبيت')
),
remainderRows AS (
    SELECT
        m.Id,
        m.StudentId,
        m.PlanId,
        m.SurahId,
        m.FromAyahNumber,
        m.ToAyahNumber,
        CAST(m.PlanDate AS date) AS PlanDate,
        m.Status
    FROM StudentPlanMemorizing m
    WHERE m.Status IN (N'منتظر التسميع', N'قيد الانتظار', N'اعادة تسميع', N'لم يتم الحفظ')
),
pairs AS (
    SELECT
        p.StudentId,
        p.PlanId,
        p.SurahId,
        p.Id AS PassId,
        p.FromAyahNumber AS PassFrom,
        p.ToAyahNumber AS PassTo,
        p.PlanDate AS PassDate,
        r.Id AS RemainderId,
        r.FromAyahNumber AS RemFrom,
        r.ToAyahNumber AS RemTo,
        r.PlanDate AS RemainderDate
    FROM passRows p
    INNER JOIN remainderRows r
        ON r.StudentId = p.StudentId
       AND r.PlanId = p.PlanId
       AND r.SurahId = p.SurahId
       AND r.FromAyahNumber = p.ToAyahNumber + 1
       AND r.PlanDate > p.PlanDate
)
SELECT
    'MEMORIZING' AS Kind,
    pr.StudentId,
    pr.PlanId,
    s.NameAr AS SurahName,
    pr.PassId,
    pr.PassFrom,
    pr.PassTo,
    pr.PassDate,
    pr.RemainderId,
    pr.RemFrom,
    pr.RemTo,
    pr.RemainderDate,
    (
        SELECT COUNT(*)
        FROM StudentPlanMemorizing c
        WHERE c.StudentId = pr.StudentId
          AND c.PlanId = pr.PlanId
          AND c.SurahId <> pr.SurahId
          AND CAST(c.PlanDate AS date) = pr.PassDate
          AND c.Status NOT IN (N'تم الحفظ', N'تم', N'قيد الانتظار في التثبيت')
    ) AS CompetingRowsOnPassDate
FROM pairs pr
LEFT JOIN QuranSurah s ON s.Id = pr.SurahId
ORDER BY pr.StudentId, pr.PlanId, pr.PassId;

PRINT N'--- Preview revise pairs (same pattern) ---';

;WITH passRows AS (
    SELECT
        m.Id,
        m.StudentId,
        m.PlanId,
        m.SurahId,
        m.FromAyahNumber,
        m.ToAyahNumber,
        CAST(m.PlanDate AS date) AS PlanDate
    FROM StudentPlanRevise m
    WHERE m.Status IN (N'تم الحفظ', N'تم', N'قيد الانتظار في التثبيت')
),
remainderRows AS (
    SELECT
        m.Id,
        m.StudentId,
        m.PlanId,
        m.SurahId,
        m.FromAyahNumber,
        m.ToAyahNumber,
        CAST(m.PlanDate AS date) AS PlanDate
    FROM StudentPlanRevise m
    WHERE m.Status IN (N'منتظر التسميع', N'قيد الانتظار', N'اعادة تسميع', N'لم يتم الحفظ')
),
pairs AS (
    SELECT
        p.StudentId,
        p.PlanId,
        p.SurahId,
        p.Id AS PassId,
        p.FromAyahNumber AS PassFrom,
        p.ToAyahNumber AS PassTo,
        p.PlanDate AS PassDate,
        r.Id AS RemainderId,
        r.FromAyahNumber AS RemFrom,
        r.ToAyahNumber AS RemTo,
        r.PlanDate AS RemainderDate
    FROM passRows p
    INNER JOIN remainderRows r
        ON r.StudentId = p.StudentId
       AND r.PlanId = p.PlanId
       AND r.SurahId = p.SurahId
       AND r.FromAyahNumber = p.ToAyahNumber + 1
       AND r.PlanDate > p.PlanDate
)
SELECT
    'REVISE' AS Kind,
    pr.StudentId,
    pr.PlanId,
    s.NameAr AS SurahName,
    pr.PassId,
    pr.PassFrom,
    pr.PassTo,
    pr.PassDate,
    pr.RemainderId,
    pr.RemFrom,
    pr.RemTo,
    pr.RemainderDate,
    (
        SELECT COUNT(*)
        FROM StudentPlanRevise c
        WHERE c.StudentId = pr.StudentId
          AND c.PlanId = pr.PlanId
          AND c.SurahId <> pr.SurahId
          AND CAST(c.PlanDate AS date) = pr.PassDate
          AND c.Status NOT IN (N'تم الحفظ', N'تم', N'قيد الانتظار في التثبيت')
    ) AS CompetingRowsOnPassDate
FROM pairs pr
LEFT JOIN QuranSurah s ON s.Id = pr.SurahId
ORDER BY pr.StudentId, pr.PlanId, pr.PassId;

------------------------------------------------------------
-- Part 2: APPLY FIX (memorizing + revise)
-- Review Part 1 first, then run this block.
------------------------------------------------------------
BEGIN TRAN;

;WITH passRows AS (
    SELECT
        m.Id,
        m.StudentId,
        m.PlanId,
        m.SurahId,
        m.ToAyahNumber,
        CAST(m.PlanDate AS date) AS PlanDate
    FROM StudentPlanMemorizing m
    WHERE m.Status IN (N'تم الحفظ', N'تم', N'قيد الانتظار في التثبيت')
),
remainderRows AS (
    SELECT
        m.Id,
        m.StudentId,
        m.PlanId,
        m.SurahId,
        m.FromAyahNumber,
        CAST(m.PlanDate AS date) AS PlanDate
    FROM StudentPlanMemorizing m
    WHERE m.Status IN (N'منتظر التسميع', N'قيد الانتظار', N'اعادة تسميع', N'لم يتم الحفظ')
),
pairs AS (
    SELECT
        p.StudentId,
        p.PlanId,
        p.SurahId,
        p.Id AS PassId,
        p.PlanDate AS PassDate,
        r.Id AS RemainderId,
        r.PlanDate AS RemainderDate
    FROM passRows p
    INNER JOIN remainderRows r
        ON r.StudentId = p.StudentId
       AND r.PlanId = p.PlanId
       AND r.SurahId = p.SurahId
       AND r.FromAyahNumber = p.ToAyahNumber + 1
       AND r.PlanDate > p.PlanDate
)
SELECT *
INTO #MemPairs
FROM pairs;

-- 2a) Bump competing surahs on the pass day → old remainder date
UPDATE c
SET c.PlanDate = p.RemainderDate,
    c.PlanEndDate = p.RemainderDate
FROM StudentPlanMemorizing c
INNER JOIN #MemPairs p
    ON c.StudentId = p.StudentId
   AND c.PlanId = p.PlanId
   AND c.SurahId <> p.SurahId
   AND CAST(c.PlanDate AS date) = p.PassDate
WHERE c.Status NOT IN (N'تم الحفظ', N'تم', N'قيد الانتظار في التثبيت');

PRINT CONCAT(N'Memorizing competitors bumped: ', @@ROWCOUNT);

-- 2b) Move remainder onto the pass day
UPDATE m
SET m.PlanDate = p.PassDate,
    m.PlanEndDate = p.PassDate
FROM StudentPlanMemorizing m
INNER JOIN #MemPairs p ON m.Id = p.RemainderId;

PRINT CONCAT(N'Memorizing remainders moved: ', @@ROWCOUNT);

-- Revise: same pattern
;WITH passRows AS (
    SELECT
        m.Id,
        m.StudentId,
        m.PlanId,
        m.SurahId,
        m.ToAyahNumber,
        CAST(m.PlanDate AS date) AS PlanDate
    FROM StudentPlanRevise m
    WHERE m.Status IN (N'تم الحفظ', N'تم', N'قيد الانتظار في التثبيت')
),
remainderRows AS (
    SELECT
        m.Id,
        m.StudentId,
        m.PlanId,
        m.SurahId,
        m.FromAyahNumber,
        CAST(m.PlanDate AS date) AS PlanDate
    FROM StudentPlanRevise m
    WHERE m.Status IN (N'منتظر التسميع', N'قيد الانتظار', N'اعادة تسميع', N'لم يتم الحفظ')
),
pairs AS (
    SELECT
        p.StudentId,
        p.PlanId,
        p.SurahId,
        p.Id AS PassId,
        p.PlanDate AS PassDate,
        r.Id AS RemainderId,
        r.PlanDate AS RemainderDate
    FROM passRows p
    INNER JOIN remainderRows r
        ON r.StudentId = p.StudentId
       AND r.PlanId = p.PlanId
       AND r.SurahId = p.SurahId
       AND r.FromAyahNumber = p.ToAyahNumber + 1
       AND r.PlanDate > p.PlanDate
)
SELECT *
INTO #RevPairs
FROM pairs;

UPDATE c
SET c.PlanDate = p.RemainderDate,
    c.PlanEndDate = p.RemainderDate
FROM StudentPlanRevise c
INNER JOIN #RevPairs p
    ON c.StudentId = p.StudentId
   AND c.PlanId = p.PlanId
   AND c.SurahId <> p.SurahId
   AND CAST(c.PlanDate AS date) = p.PassDate
WHERE c.Status NOT IN (N'تم الحفظ', N'تم', N'قيد الانتظار في التثبيت');

PRINT CONCAT(N'Revise competitors bumped: ', @@ROWCOUNT);

UPDATE m
SET m.PlanDate = p.PassDate,
    m.PlanEndDate = p.PassDate
FROM StudentPlanRevise m
INNER JOIN #RevPairs p ON m.Id = p.RemainderId;

PRINT CONCAT(N'Revise remainders moved: ', @@ROWCOUNT);

-- Extend plan end date if bumped dates went past it
UPDATE pl
SET pl.PlanToDate = x.MaxDate
FROM StudentPlan pl
INNER JOIN (
    SELECT PlanId, MAX(CAST(PlanDate AS date)) AS MaxDate
    FROM (
        SELECT PlanId, PlanDate FROM StudentPlanMemorizing
        UNION ALL
        SELECT PlanId, PlanDate FROM StudentPlanRevise
    ) u
    GROUP BY PlanId
) x ON x.PlanId = pl.Id
WHERE x.MaxDate > pl.PlanToDate;

PRINT CONCAT(N'Plans with PlanToDate extended: ', @@ROWCOUNT);

-- Post-check: these should be empty after a good fix
SELECT N'Remaining memorizing collisions' AS CheckName, COUNT(*) AS Cnt
FROM #MemPairs p
INNER JOIN StudentPlanMemorizing r ON r.Id = p.RemainderId
WHERE CAST(r.PlanDate AS date) > p.PassDate
UNION ALL
SELECT N'Remaining revise collisions', COUNT(*)
FROM #RevPairs p
INNER JOIN StudentPlanRevise r ON r.Id = p.RemainderId
WHERE CAST(r.PlanDate AS date) > p.PassDate;

DROP TABLE #MemPairs;
DROP TABLE #RevPairs;

-- Inspect student 1409 after fix (optional)
SELECT
    m.Id,
    m.StudentId,
    m.PlanId,
    CAST(m.PlanDate AS date) AS PlanDate,
    s.NameAr,
    m.FromAyahNumber,
    m.ToAyahNumber,
    m.Status
FROM StudentPlanMemorizing m
JOIN QuranSurah s ON s.Id = m.SurahId
WHERE m.StudentId = 1409
ORDER BY m.PlanId, m.PlanDate, m.FromAyahNumber, m.Id;

-- COMMIT;
-- ROLLBACK;
