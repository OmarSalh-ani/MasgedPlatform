/*
  One-time data fix for student 1409 / plan 211
  after partial pass left الملك remainder AFTER التحريم on the calendar.

  Desired after fix:
    2026-08-02: الملك 1-5 (تم الحفظ), الملك 6-30 (منتظر)
    2026-08-03: التحريم, الطلاق, التغابن (and later surahs) pending
*/
SET NOCOUNT ON;
DECLARE @StudentId INT = 1409;
DECLARE @PlanId INT = 211;
DECLARE @MulkId INT = 67;
DECLARE @Day DATE = '2026-08-02';
DECLARE @NextDay DATE = '2026-08-03';

BEGIN TRAN;

-- Remainder of الملك stays on the original day
UPDATE StudentPlanMemorizing
SET PlanDate = @Day,
    PlanEndDate = @Day
WHERE StudentId = @StudentId
  AND PlanId = @PlanId
  AND SurahId = @MulkId
  AND FromAyahNumber = 6
  AND ToAyahNumber = 30
  AND Status = N'منتظر التسميع';

-- Other surahs that were competing on that day move to next work day
UPDATE StudentPlanMemorizing
SET PlanDate = @NextDay,
    PlanEndDate = @NextDay
WHERE StudentId = @StudentId
  AND PlanId = @PlanId
  AND SurahId <> @MulkId
  AND PlanDate = @Day
  AND Status <> N'تم الحفظ';

-- Verify
SELECT
    m.Id,
    m.PlanDate,
    s.NameAr,
    m.FromAyahNumber,
    m.ToAyahNumber,
    m.Status
FROM StudentPlanMemorizing m
JOIN QuranSurah s ON s.Id = m.SurahId
WHERE m.StudentId = @StudentId AND m.PlanId = @PlanId
ORDER BY m.PlanDate, m.FromAyahNumber, m.Id;

-- COMMIT;
-- ROLLBACK;
