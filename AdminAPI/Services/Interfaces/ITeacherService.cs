using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Teachers;

namespace AdminAPI.Services.Interfaces;

public interface ITeacherService
{
    Task<PagedResultDto<TeacherListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<TeacherCardPrintDto> GetCardPrintAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default);
}
