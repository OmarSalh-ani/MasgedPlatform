using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IHeroSlideRepository
{
    Task<(List<HeroSlide> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<HeroSlide?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<HeroSlide> AddAsync(HeroSlide entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
