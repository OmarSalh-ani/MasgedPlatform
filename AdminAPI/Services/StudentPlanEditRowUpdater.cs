using AdminAPI.Data;
using AdminAPI.DTOs.StudentPlan;
using AdminAPI.Models;

namespace AdminAPI.Services;

public static class StudentPlanEditRowUpdater
{
    public static void UpdateEditRow(
        AdminDbContext db,
        int studentId,
        EditPlanRowInputDto row,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now)
    {
        var planType = string.IsNullOrEmpty(row.PlanType) ? StudentPlanConstants.TypeMemorizing : row.PlanType;
        var wantMemorizing = planType is StudentPlanConstants.TypeMemorizing or StudentPlanConstants.TypeBoth;
        var wantRevise = planType is StudentPlanConstants.TypeRevise or StudentPlanConstants.TypeBoth;

        if (row.Key.StartsWith("memorizing_") && int.TryParse(row.Key["memorizing_".Length..], out var memId))
            UpdateFromMemorizing(db, studentId, memId, row, level, planStart, planEnd, now, wantMemorizing, wantRevise);
        else if (row.Key.StartsWith("revise_") && int.TryParse(row.Key["revise_".Length..], out var revId))
            UpdateFromRevise(db, studentId, revId, row, level, planStart, planEnd, now, wantMemorizing, wantRevise);
    }

    private static void UpdateFromMemorizing(
        AdminDbContext db,
        int studentId,
        int memId,
        EditPlanRowInputDto row,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        bool wantMemorizing,
        bool wantRevise)
    {
        var memEnt = db.StudentPlanMemorizings.Find(memId);
        if (memEnt is null)
            return;

        if (wantMemorizing && !wantRevise)
            ApplyMemorizing(memEnt, row, level, planStart, planEnd);
        else if (wantRevise && !wantMemorizing)
        {
            var planId = memEnt.PlanId;
            db.StudentPlanMemorizings.Remove(memEnt);
            db.StudentPlanRevises.Add(CreateRevise(studentId, planId, row, level, planStart, planEnd, now));
        }
        else
        {
            ApplyMemorizing(memEnt, row, level, planStart, planEnd);
            db.StudentPlanRevises.Add(CreateRevise(studentId, memEnt.PlanId, row, level, planStart, planEnd, now));
        }
    }

    private static void UpdateFromRevise(
        AdminDbContext db,
        int studentId,
        int revId,
        EditPlanRowInputDto row,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        bool wantMemorizing,
        bool wantRevise)
    {
        var revEnt = db.StudentPlanRevises.Find(revId);
        if (revEnt is null)
            return;

        if (wantRevise && !wantMemorizing)
            ApplyRevise(revEnt, row, level, planStart, planEnd);
        else if (wantMemorizing && !wantRevise)
        {
            var planId = revEnt.PlanId;
            db.StudentPlanRevises.Remove(revEnt);
            db.StudentPlanMemorizings.Add(CreateMemorizing(studentId, planId, row, level, planStart, planEnd, now));
        }
        else
        {
            ApplyRevise(revEnt, row, level, planStart, planEnd);
            db.StudentPlanMemorizings.Add(CreateMemorizing(studentId, revEnt.PlanId, row, level, planStart, planEnd, now));
        }
    }

    private static void ApplyMemorizing(
        StudentPlanMemorizing ent,
        EditPlanRowInputDto row,
        string level,
        DateTime planStart,
        DateTime planEnd)
    {
        ent.MemorizationLevel = level;
        ent.SurahId = row.SurahId;
        ent.FromAyahNumber = row.FromAyahNumber;
        ent.ToAyahNumber = row.ToAyahNumber;
        ent.PlanDate = planStart;
        ent.PlanEndDate = planEnd;
    }

    private static void ApplyRevise(
        StudentPlanRevise ent,
        EditPlanRowInputDto row,
        string level,
        DateTime planStart,
        DateTime planEnd)
    {
        ent.MemorizationLevel = level;
        ent.SurahId = row.SurahId;
        ent.FromAyahNumber = row.FromAyahNumber;
        ent.ToAyahNumber = row.ToAyahNumber;
        ent.PlanDate = planStart;
        ent.PlanEndDate = planEnd;
    }

    private static StudentPlanMemorizing CreateMemorizing(
        int studentId,
        int planId,
        EditPlanRowInputDto row,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now) =>
        new()
        {
            StudentId = studentId,
            PlanId = planId,
            MemorizationLevel = level,
            SurahId = row.SurahId,
            FromAyahNumber = row.FromAyahNumber,
            ToAyahNumber = row.ToAyahNumber,
            PlanDate = planStart,
            PlanEndDate = planEnd,
            CreatedAt = now,
        };

    private static StudentPlanRevise CreateRevise(
        int studentId,
        int planId,
        EditPlanRowInputDto row,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now) =>
        new()
        {
            StudentId = studentId,
            PlanId = planId,
            MemorizationLevel = level,
            SurahId = row.SurahId,
            FromAyahNumber = row.FromAyahNumber,
            ToAyahNumber = row.ToAyahNumber,
            PlanDate = planStart,
            PlanEndDate = planEnd,
            CreatedAt = now,
        };
}
