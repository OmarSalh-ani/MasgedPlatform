using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.FilesManager;

namespace AdminAPI.Services.Interfaces;

public interface IFilesManagerService
{
    Task<PagedResultDto<FilesManagerListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<FilesManagerDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<FilesManagerDto> CreateAsync(
        SaveFilesManagerRequestDto request,
        CancellationToken cancellationToken = default);

    Task<FilesManagerDto> UpdateAsync(
        int id,
        SaveFilesManagerRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default);
}
