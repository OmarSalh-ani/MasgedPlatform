using AdminAPI.Data;
using AdminAPI.DTOs.StudentPlan;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;

namespace AdminAPI.Services;

public partial class StudentPlanService(
    AdminDbContext db,
    IStudentPlanRepository repository,
    ICurrentUserContext currentUser,
    IValidator<CreateStudentPlanRequestDto> createValidator,
    IValidator<SaveStudentPlanRequestDto> saveValidator,
    IValidator<UpdateStudentPlanItemRequestDto> updateValidator) : IStudentPlanService
{
    public async Task<StudentPlanFormDataDto> GetFormDataAsync(CancellationToken cancellationToken = default)
    {
        var surahs = await repository.GetSurahsAsync(cancellationToken);
        var circles = await repository.GetCirclesAsync(
            currentUser.IsGirlTeacher,
            currentUser.IsAdmin,
            currentUser.TeacherId,
            cancellationToken);
        var students = await repository.GetStudentsAsync(
            currentUser.IsGirlTeacher,
            currentUser.IsAdmin,
            currentUser.TeacherId,
            cancellationToken);

        return new StudentPlanFormDataDto
        {
            Circles = circles,
            Students = students,
            Surahs = StudentPlanSurahExpander.GetExpandedSurahsList(surahs),
            MemorizationLevels = StudentPlanConstants.MemorizationLevels.ToList(),
            PlanTypes = StudentPlanConstants.PlanTypes.ToList(),
            CanModify = currentUser.CanModify,
        };
    }

    public async Task<List<StudentPlanAyahDto>> GetAyahsAsync(
        int surahId,
        CancellationToken cancellationToken = default)
    {
        var ayahs = await repository.GetAyahNumbersAsync(surahId, cancellationToken);
        return ayahs.Select(a => new StudentPlanAyahDto { AyahNumber = a }).ToList();
    }

    public async Task<StudentPlanResolveDto> ResolveAsync(
        int studentId,
        int? planId,
        string? editKey,
        CancellationToken cancellationToken = default)
    {
        var student = await repository.GetStudentAsync(studentId, cancellationToken)
            ?? throw new KeyNotFoundException("الطالب غير موجود");

        if (!planId.HasValue && !string.IsNullOrEmpty(editKey))
            planId = await repository.ResolvePlanIdFromEditKeyAsync(studentId, editKey, cancellationToken);

        var plans = await repository.GetPlansForStudentAsync(studentId, cancellationToken);
        if (!planId.HasValue && plans.Count > 0)
        {
            var today = KuwaitTime.Today;
            planId = plans.FirstOrDefault(p => today >= p.PlanFromDate && today <= p.PlanToDate)?.Id
                ?? plans.First().Id;
        }

        return new StudentPlanResolveDto
        {
            StudentId = studentId,
            StudentName = StudentPlanMapper.GetStudentDisplayName(student),
            PlanId = planId,
            ShouldCreateNew = !planId.HasValue && plans.Count == 0,
        };
    }

    public async Task<StudentPlanDetailDto> GetPlanDetailAsync(
        int studentId,
        int planId,
        CancellationToken cancellationToken = default)
    {
        var student = await repository.GetStudentAsync(studentId, cancellationToken)
            ?? throw new KeyNotFoundException("الطالب غير موجود");
        var plan = await repository.GetPlanAsync(planId, cancellationToken)
            ?? throw new KeyNotFoundException("الخطة غير موجودة");
        if (plan.StudentId != studentId)
            throw new InvalidOperationException("الخطة لا تخص هذا الطالب");

        var plans = await repository.GetPlansForStudentAsync(studentId, cancellationToken);
        var mem = await repository.GetMemorizingsAsync(studentId, planId, cancellationToken);
        var rev = await repository.GetRevisesAsync(studentId, planId, cancellationToken);

        return new StudentPlanDetailDto
        {
            StudentId = studentId,
            StudentName = StudentPlanMapper.GetStudentDisplayName(student),
            PlanId = planId,
            PlanName = plan.Name,
            Plans = StudentPlanMapper.MapPlanOptions(plans),
            Header = StudentPlanMapper.BuildHeader(mem, rev, KuwaitTime.Today),
            Items = StudentPlanMapper.MapItems(mem, rev),
            CanModify = currentUser.CanModify,
        };
    }

    public async Task<StudentPlanEditPrefillDto?> GetEditPrefillAsync(
        string editKey,
        CancellationToken cancellationToken = default)
    {
        if (editKey.StartsWith("memorizing_") && int.TryParse(editKey["memorizing_".Length..], out var memId))
        {
            var ent = await repository.GetMemorizingByIdAsync(memId, cancellationToken);
            if (ent is null) return null;
            return new StudentPlanEditPrefillDto
            {
                MemorizationLevel = ent.MemorizationLevel,
                PlanStartDate = ent.PlanDate.ToString("yyyy-MM-dd"),
                PlanEndDate = ent.PlanDate.ToString("yyyy-MM-dd"),
                SurahId = ent.SurahId,
                FromAyahNumber = ent.FromAyahNumber,
                ToAyahNumber = ent.ToAyahNumber,
                PlanType = StudentPlanConstants.TypeMemorizing,
                PlanId = ent.PlanId,
            };
        }

        if (editKey.StartsWith("revise_") && int.TryParse(editKey["revise_".Length..], out var revId))
        {
            var ent = await repository.GetReviseByIdAsync(revId, cancellationToken);
            if (ent is null) return null;
            return new StudentPlanEditPrefillDto
            {
                MemorizationLevel = ent.MemorizationLevel,
                PlanStartDate = ent.PlanDate.ToString("yyyy-MM-dd"),
                PlanEndDate = ent.PlanDate.ToString("yyyy-MM-dd"),
                SurahId = ent.SurahId,
                FromAyahNumber = ent.FromAyahNumber,
                ToAyahNumber = ent.ToAyahNumber,
                PlanType = StudentPlanConstants.TypeRevise,
                PlanId = ent.PlanId,
            };
        }

        return null;
    }
}
