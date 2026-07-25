using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.QuranCircles;

namespace AdminAPI.Services.Interfaces;

public interface IQuranCircleService
{
    Task<PagedResultDto<QuranCircleListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        int? teacherId,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default);

    Task<QuranCircleDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<TeacherOptionDto>> GetTeachersAsync(CancellationToken cancellationToken = default);
    Task<QuranCircleDto> CreateAsync(SaveQuranCircleRequestDto request, CancellationToken cancellationToken = default);
    Task<QuranCircleDto> UpdateAsync(int id, SaveQuranCircleRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
