using AdminAPI.Data;
using AdminAPI.Models;

namespace AdminAPI.Services;

public static class StudentPlanItemWriter
{
    public static void AddPlanRows(
        AdminDbContext db,
        int studentId,
        int planId,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        IEnumerable<ExpandedPlanRow> rows)
    {
        foreach (var row in rows)
        {
            if (row.PlanType == StudentPlanConstants.TypeRevise)
            {
                db.StudentPlanRevises.Add(CreateRevise(studentId, planId, level, planStart, planEnd, now, row));
                continue;
            }

            if (row.PlanType == StudentPlanConstants.TypeBoth)
            {
                db.StudentPlanMemorizings.Add(CreateMemorizing(studentId, planId, level, planStart, planEnd, now, row));
                db.StudentPlanRevises.Add(CreateRevise(studentId, planId, level, planStart, planEnd, now, row));
                continue;
            }

            db.StudentPlanMemorizings.Add(CreateMemorizing(studentId, planId, level, planStart, planEnd, now, row));
        }
    }

    private static StudentPlanMemorizing CreateMemorizing(
        int studentId,
        int planId,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        ExpandedPlanRow row) =>
        new()
        {
            StudentId = studentId,
            PlanId = planId,
            MemorizationLevel = level,
            SurahId = row.SurahId,
            FromAyahNumber = row.FromAyah,
            ToAyahNumber = row.ToAyah,
            PlanDate = planStart,
            PlanEndDate = planEnd,
            CreatedAt = now,
        };

    private static StudentPlanRevise CreateRevise(
        int studentId,
        int planId,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        ExpandedPlanRow row) =>
        new()
        {
            StudentId = studentId,
            PlanId = planId,
            MemorizationLevel = level,
            SurahId = row.SurahId,
            FromAyahNumber = row.FromAyah,
            ToAyahNumber = row.ToAyah,
            PlanDate = planStart,
            PlanEndDate = planEnd,
            CreatedAt = now,
        };
}
