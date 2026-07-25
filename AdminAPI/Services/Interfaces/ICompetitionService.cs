using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Competitions;

namespace AdminAPI.Services.Interfaces;

public interface ICompetitionService
{
    Task<PagedResultDto<CompetitionListItemDto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CompetitionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<CompetitionDto> CreateAsync(
        SaveCompetitionRequestDto request,
        CancellationToken cancellationToken = default);

    Task<CompetitionDto> UpdateAsync(
        int id,
        SaveCompetitionRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        bool deleteImageFile,
        CancellationToken cancellationToken = default);
}
