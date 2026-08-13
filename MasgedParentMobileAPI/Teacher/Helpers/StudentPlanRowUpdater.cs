using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;

namespace MasgedTeacherMobileAPI.Helpers;

public static class StudentPlanRowUpdater
{
    public const string TypeMemorizing = "حفظ";
    public const string TypeRevise = "مراجعة";

    public static string? ValidateRequest(UpdatePlanRowRequestDto request)
    {
        var isManual = !string.IsNullOrWhiteSpace(request.SurahName);

        if (!isManual && request.SurahId <= 0)
            return "يرجى اختيار السورة";

        if (isManual && string.IsNullOrWhiteSpace(request.SurahName))
            return "يرجى إدخال اسم السورة";

        if (request.FromAyahNumber <= 0 || request.ToAyahNumber <= 0)
            return "يرجى إدخال نطاق آيات صحيح";

        if (request.FromAyahNumber > request.ToAyahNumber)
            return "نطاق الآيات غير صحيح";

        var planType = NormalizePlanType(request.PlanType);
        if (planType is not TypeMemorizing and not TypeRevise)
            return "نوع الخطة غير صالح";

        return null;
    }

    public static bool UpdateRow(
        AppDbContext db,
        int studentId,
        string rowKey,
        UpdatePlanRowRequestDto request,
        DateTime planStart,
        DateTime planEnd,
        DateTime now)
    {
        var planType = NormalizePlanType(request.PlanType);

        if (rowKey.StartsWith("memorizing_") && int.TryParse(rowKey["memorizing_".Length..], out var memId))
        {
            var memEnt = db.StudentPlanMemorizings
                .FirstOrDefault(x => x.Id == memId && x.StudentId == studentId);
            if (memEnt is null)
                return false;

            if (planType == TypeMemorizing)
            {
                ApplyMemorizing(memEnt, request, planStart, planEnd);
                return true;
            }

            ConvertMemorizingToRevise(db, memEnt, studentId, request, planStart, planEnd, now);
            return true;
        }

        if (rowKey.StartsWith("revise_") && int.TryParse(rowKey["revise_".Length..], out var revId))
        {
            var revEnt = db.StudentPlanRevises
                .FirstOrDefault(x => x.Id == revId && x.StudentId == studentId);
            if (revEnt is null)
                return false;

            if (planType == TypeRevise)
            {
                ApplyRevise(revEnt, request, planStart, planEnd);
                return true;
            }

            ConvertReviseToMemorizing(db, revEnt, studentId, request, planStart, planEnd, now);
            return true;
        }

        return false;
    }

    private static string NormalizePlanType(string? planType) =>
        string.IsNullOrWhiteSpace(planType) ? TypeMemorizing : planType.Trim();

    private static void ApplyMemorizing(
        StudentPlanMemorizing ent,
        UpdatePlanRowRequestDto request,
        DateTime planStart,
        DateTime planEnd)
    {
        ent.MemorizationLevel = ManualPlanRowHelper.ResolveLevel(
            request.SurahName,
            ent.MemorizationLevel);
        ent.SurahId = ResolveSurahId(request);
        ent.FromAyahNumber = request.FromAyahNumber;
        ent.ToAyahNumber = request.ToAyahNumber;
        ent.PlanDate = planStart;
        ent.PlanEndDate = planEnd;
    }

    private static void ApplyRevise(
        StudentPlanRevise ent,
        UpdatePlanRowRequestDto request,
        DateTime planStart,
        DateTime planEnd)
    {
        ent.MemorizationLevel = ManualPlanRowHelper.ResolveLevel(
            request.SurahName,
            ent.MemorizationLevel);
        ent.SurahId = ResolveSurahId(request);
        ent.FromAyahNumber = request.FromAyahNumber;
        ent.ToAyahNumber = request.ToAyahNumber;
        ent.PlanDate = planStart;
        ent.PlanEndDate = planEnd;
    }

    private static int ResolveSurahId(UpdatePlanRowRequestDto request) =>
        string.IsNullOrWhiteSpace(request.SurahName)
            ? request.SurahId
            : ManualPlanRowHelper.PlaceholderSurahId;

    private static void ConvertMemorizingToRevise(
        AppDbContext db,
        StudentPlanMemorizing memEnt,
        int studentId,
        UpdatePlanRowRequestDto request,
        DateTime planStart,
        DateTime planEnd,
        DateTime now)
    {
        var planId = memEnt.PlanId;
        var level = ManualPlanRowHelper.ResolveLevel(request.SurahName, memEnt.MemorizationLevel);
        var status = memEnt.Status;
        db.StudentPlanMemorizings.Remove(memEnt);
        db.StudentPlanRevises.Add(CreateRevise(
            studentId, planId, request, level, planStart, planEnd, now, status));
    }

    private static void ConvertReviseToMemorizing(
        AppDbContext db,
        StudentPlanRevise revEnt,
        int studentId,
        UpdatePlanRowRequestDto request,
        DateTime planStart,
        DateTime planEnd,
        DateTime now)
    {
        var planId = revEnt.PlanId;
        var level = ManualPlanRowHelper.ResolveLevel(request.SurahName, revEnt.MemorizationLevel);
        var status = revEnt.Status;
        db.StudentPlanRevises.Remove(revEnt);
        db.StudentPlanMemorizings.Add(CreateMemorizing(
            studentId, planId, request, level, planStart, planEnd, now, status));
    }

    private static StudentPlanMemorizing CreateMemorizing(
        int studentId,
        int planId,
        UpdatePlanRowRequestDto request,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        string? status) =>
        new()
        {
            StudentId = studentId,
            PlanId = planId,
            MemorizationLevel = level,
            SurahId = ResolveSurahId(request),
            FromAyahNumber = request.FromAyahNumber,
            ToAyahNumber = request.ToAyahNumber,
            PlanDate = planStart,
            PlanEndDate = planEnd,
            CreatedAt = now,
            Status = status
        };

    private static StudentPlanRevise CreateRevise(
        int studentId,
        int planId,
        UpdatePlanRowRequestDto request,
        string level,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        string? status) =>
        new()
        {
            StudentId = studentId,
            PlanId = planId,
            MemorizationLevel = level,
            SurahId = ResolveSurahId(request),
            FromAyahNumber = request.FromAyahNumber,
            ToAyahNumber = request.ToAyahNumber,
            PlanDate = planStart,
            PlanEndDate = planEnd,
            CreatedAt = now,
            Status = status
        };
}
