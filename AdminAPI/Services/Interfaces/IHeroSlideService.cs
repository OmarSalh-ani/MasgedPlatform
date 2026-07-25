using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.HeroSlides;

namespace AdminAPI.Services.Interfaces;

public interface IHeroSlideService
{
    Task<PagedResultDto<HeroSlideListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<HeroSlideDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<HeroSlideDto> CreateAsync(
        SaveHeroSlideRequestDto request,
        CancellationToken cancellationToken = default);

    Task<HeroSlideDto> UpdateAsync(
        int id,
        SaveHeroSlideRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        bool deleteImageFile,
        CancellationToken cancellationToken = default);
}
