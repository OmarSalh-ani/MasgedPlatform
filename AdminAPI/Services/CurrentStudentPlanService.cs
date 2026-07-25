using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.CurrentStudentsPlans;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class CurrentStudentPlanService(
    ICurrentStudentPlanRepository repository,
    IWorkDayService workDayService) : ICurrentStudentPlanService
{
    public async Task<PagedResultDto<CurrentStudentPlanListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        int? studentId = null,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var today = KuwaitTime.Today;
        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);
        var (plans, totalCount) = await repository.GetPagedAsync(page, pageSize, studentId, cancellationToken);
        var items = plans.Select(p => MapToListItem(p, workDayNumbers, today)).ToList();
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        return new PagedResultDto<CurrentStudentPlanListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize),
        };
    }

    public async Task<PagedResultDto<CurrentStudentPlanStudentLookupDto>> GetStudentsAsync(
        CurrentStudentPlanStudentLookupFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var page = filters.PageNumber < 1 ? 1 : filters.PageNumber;
        var size = filters.PageSize < 1 ? 20 : filters.PageSize;
        var (rows, totalCount) = await repository.GetStudentLookupAsync(
            filters.Search,
            page,
            size,
            cancellationToken);

        var duplicateNames = await repository.GetDuplicateStudentNameSetAsync(filters.Search, cancellationToken);

        var items = rows.Select(row => new CurrentStudentPlanStudentLookupDto
        {
            Id = row.Id,
            Name = row.Name,
            Label = FormatStudentLabel(row.Name, row.Id, duplicateNames),
        }).ToList();

        return new PagedResultDto<CurrentStudentPlanStudentLookupDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)size),
        };
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        repository.DeleteWithRelatedAsync(id, cancellationToken);

    private static CurrentStudentPlanListItemDto MapToListItem(
        StudentPlan plan,
        IReadOnlyList<int> workDayNumbers,
        DateTime today)
    {
        var (totalDays, elapsedDays, remainingDays) = StudentPlanDayCalculator.Calculate(
            plan.PlanFromDate,
            plan.PlanToDate,
            workDayNumbers,
            today);

        return new CurrentStudentPlanListItemDto
        {
            Id = plan.Id,
            StudentId = plan.StudentId,
            StudentName = plan.RegisterForm?.StudentName ?? "—",
            PlanName = plan.Name,
            FromDate = plan.PlanFromDate,
            ToDate = plan.PlanToDate,
            CreatedAt = plan.CreatedAt,
            TotalDays = totalDays,
            ElapsedDays = elapsedDays,
            RemainingDays = remainingDays,
            CircleName = plan.RegisterForm?.QuranCircle?.Name ?? "—",
        };
    }

    private static string FormatStudentLabel(string name, int id, HashSet<string> duplicateNames)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrEmpty(trimmedName))
            return $"#{id}";

        return duplicateNames.Contains(trimmedName) ? $"{trimmedName} (#{id})" : trimmedName;
    }
}
