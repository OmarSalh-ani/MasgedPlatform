using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Expensives;

namespace AdminAPI.Services.Interfaces;

public interface IExpensiveService
{
    Task<PagedResultDto<ExpensiveListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ExpensiveSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<ExpensiveDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ExpensiveDto> CreateAsync(SaveExpensiveRequestDto request, CancellationToken cancellationToken = default);

    Task<ExpensiveDto> UpdateAsync(int id, SaveExpensiveRequestDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default);

    Task DeleteAttachmentAsync(int id, string fileName, CancellationToken cancellationToken = default);

    Task<(string Path, string FileName)> GetAttachmentFileAsync(
        int id,
        string fileName,
        CancellationToken cancellationToken = default);
}
