using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Expensives;

namespace AdminAPI.Services;

public partial class ExpensiveService
{
    public async Task<PagedResultDto<ExpensiveListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetListAsync(currentUser.IsGirlTeacher, cancellationToken);
        var items = entities.Select(entity => mapper.Map<ExpensiveListItemDto>(entity)).ToList();
        return ToPagedResult(items, pageNumber, pageSize);
    }

    public async Task<ExpensiveSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetListAsync(currentUser.IsGirlTeacher, cancellationToken);
        var totalCount = entities.Count;
        var totalAmount = entities.Sum(x => x.TotalAmount);
        var now = KuwaitTime.Now;
        var thisMonthExpenses = entities
            .Where(x => x.CreatedAt.Year == now.Year && x.CreatedAt.Month == now.Month)
            .ToList();
        var thisMonthCount = thisMonthExpenses.Count;
        var thisMonthAmount = thisMonthExpenses.Sum(x => x.TotalAmount);
        var averageAmount = totalCount > 0 ? totalAmount / totalCount : 0;

        return new ExpensiveSummaryDto
        {
            TotalCount = totalCount,
            TotalAmount = totalAmount,
            ThisMonthCount = thisMonthCount,
            ThisMonthAmount = thisMonthAmount,
            AverageAmount = averageAmount,
        };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await repository.DeleteAsync(id, currentUser.IsGirlTeacher, cancellationToken);

    public async Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetListAsync(currentUser.IsGirlTeacher, cancellationToken);
        var items = entities.Select(entity => mapper.Map<ExpensiveListItemDto>(entity)).ToList();
        return ExpensiveExcelExporter.Build(items);
    }

    private static PagedResultDto<ExpensiveListItemDto> ToPagedResult(
        List<ExpensiveListItemDto> items,
        int pageNumber,
        int pageSize)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var totalCount = items.Count;
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;
        var pagedItems = pageSize <= 0
            ? items
            : items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResultDto<ExpensiveListItemDto>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize),
        };
    }
}
