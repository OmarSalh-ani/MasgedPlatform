using AdminAPI.DTOs.StudentPlan;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;

namespace AdminAPI.Services;

public partial class StudentPlanService
{
    public async Task<CreateStudentPlanResponseDto> CreatePlanAsync(
        CreateStudentPlanRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var fromDate = request.FromDate?.Date ?? KuwaitTime.Today;
        var toDate = request.ToDate?.Date ?? fromDate;
        if (toDate < fromDate)
            toDate = fromDate;

        var plan = new StudentPlan
        {
            StudentId = request.StudentId,
            Name = request.Name.Trim(),
            PlanFromDate = fromDate,
            PlanToDate = toDate,
            CreatedAt = KuwaitTime.Now,
        };

        db.StudentPlans.Add(plan);
        await repository.SaveChangesAsync(cancellationToken);

        return new CreateStudentPlanResponseDto { PlanId = plan.Id };
    }

    public async Task SavePlanAsync(
        SaveStudentPlanRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        await saveValidator.ValidateAndThrowAsync(request, cancellationToken);

        var selectedStudents = ResolveSelectedStudents(request);
        var planStart = request.PlanStartDate?.Date ?? KuwaitTime.Today;
        var planEnd = request.PlanEndDate?.Date ?? planStart;
        var level = string.IsNullOrEmpty(request.MemorizationLevel)
            ? StudentPlanConstants.DefaultLevel
            : request.MemorizationLevel;
        var now = KuwaitTime.Now;

        if (request.EditMode && request.StudentId.HasValue && request.EditRows.Count > 0)
        {
            foreach (var row in request.EditRows)
            {
                try
                {
                    StudentPlanEditRowUpdater.UpdateEditRow(
                        db,
                        request.StudentId.Value,
                        row,
                        level,
                        planStart,
                        planEnd,
                        now);
                }
                catch
                {
                    // skip invalid row — matches legacy
                }
            }
        }

        var studentToPlanId = await BuildStudentPlanMapAsync(
            request,
            selectedStudents,
            planStart,
            planEnd,
            now,
            cancellationToken);

        var expandedRows = await ExpandNewRowsAsync(request.NewRows, cancellationToken);
        foreach (var row in expandedRows)
        {
            foreach (var studentId in selectedStudents)
            {
                StudentPlanItemWriter.AddPlanRows(
                    db,
                    studentId,
                    studentToPlanId[studentId],
                    level,
                    planStart,
                    planEnd,
                    now,
                    [row]);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSingleItemAsync(
        UpdateStudentPlanItemRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var planStart = request.PlanStartDate?.Date ?? KuwaitTime.Today;
        var planEnd = request.PlanEndDate?.Date ?? planStart;
        var level = string.IsNullOrEmpty(request.MemorizationLevel)
            ? StudentPlanConstants.DefaultLevel
            : request.MemorizationLevel;

        if (request.EditKey.StartsWith("memorizing_") && int.TryParse(request.EditKey["memorizing_".Length..], out var memId))
        {
            var ent = await repository.GetMemorizingByIdAsync(memId, cancellationToken)
                ?? throw new KeyNotFoundException("البند غير موجود");
            ent.MemorizationLevel = level;
            ent.SurahId = request.SurahId;
            ent.FromAyahNumber = request.FromAyahNumber;
            ent.ToAyahNumber = request.ToAyahNumber;
            ent.PlanDate = planStart;
            ent.PlanEndDate = planEnd;
            await repository.SaveChangesAsync(cancellationToken);
            return;
        }

        if (request.EditKey.StartsWith("revise_") && int.TryParse(request.EditKey["revise_".Length..], out var revId))
        {
            var ent = await repository.GetReviseByIdAsync(revId, cancellationToken)
                ?? throw new KeyNotFoundException("البند غير موجود");
            ent.MemorizationLevel = level;
            ent.SurahId = request.SurahId;
            ent.FromAyahNumber = request.FromAyahNumber;
            ent.ToAyahNumber = request.ToAyahNumber;
            ent.PlanDate = planStart;
            ent.PlanEndDate = planEnd;
            await repository.SaveChangesAsync(cancellationToken);
            return;
        }

        throw new KeyNotFoundException("البند غير موجود");
    }

    public async Task<bool> DeleteItemAsync(string editKey, CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        if (editKey.StartsWith("memorizing_") && int.TryParse(editKey["memorizing_".Length..], out var memId))
        {
            var ent = await repository.GetMemorizingByIdAsync(memId, cancellationToken);
            if (ent is null) return false;
            db.StudentPlanMemorizings.Remove(ent);
            await repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        if (editKey.StartsWith("revise_") && int.TryParse(editKey["revise_".Length..], out var revId))
        {
            var ent = await repository.GetReviseByIdAsync(revId, cancellationToken);
            if (ent is null) return false;
            db.StudentPlanRevises.Remove(ent);
            await repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    private async Task<Dictionary<int, int>> BuildStudentPlanMapAsync(
        SaveStudentPlanRequestDto request,
        List<int> selectedStudents,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var map = new Dictionary<int, int>();
        if (request.PlanId.HasValue
            && selectedStudents.Count == 1
            && request.StudentId.HasValue
            && selectedStudents[0] == request.StudentId.Value)
        {
            map[selectedStudents[0]] = request.PlanId.Value;
            return map;
        }

        foreach (var sid in selectedStudents)
        {
            var plan = new StudentPlan
            {
                StudentId = sid,
                Name = "خطة جديدة " + planStart.ToString("yyyy-MM-dd"),
                PlanFromDate = planStart,
                PlanToDate = planEnd,
                CreatedAt = now,
            };
            db.StudentPlans.Add(plan);
            await repository.SaveChangesAsync(cancellationToken);
            map[sid] = plan.Id;
        }

        return map;
    }

    private static List<int> ResolveSelectedStudents(SaveStudentPlanRequestDto request)
    {
        if (request.StudentId.HasValue)
            return [request.StudentId.Value];

        return request.StudentIds.Where(id => id > 0).Distinct().ToList();
    }

    private async Task<List<ExpandedPlanRow>> ExpandNewRowsAsync(
        IEnumerable<PlanRowInputDto> rows,
        CancellationToken cancellationToken)
    {
        var expanded = new List<ExpandedPlanRow>();
        foreach (var row in rows)
        {
            if (row.SurahId <= 0)
                continue;

            var fromAyah = row.SurahId > 1000 ? 1 : row.FromAyahNumber;
            var toAyah = row.SurahId > 1000 ? 1 : row.ToAyahNumber;
            if (row.SurahId <= 1000 && (fromAyah <= 0 || toAyah <= 0))
                continue;

            var planType = string.IsNullOrEmpty(row.PlanType) ? StudentPlanConstants.TypeMemorizing : row.PlanType;
            var items = await StudentPlanSurahExpander.ExpandSurahIdAsync(
                db,
                row.SurahId,
                fromAyah,
                toAyah,
                planType,
                cancellationToken);
            expanded.AddRange(items);
        }

        return expanded;
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("غير مسموح بالتعديل");
    }
}
