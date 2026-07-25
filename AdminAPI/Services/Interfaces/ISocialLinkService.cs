using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.SocialLink;

namespace AdminAPI.Services.Interfaces;

public interface ISocialLinkService
{
    Task<PagedResultDto<SocialLinkListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<SocialLinkDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<SocialLinkDto> CreateAsync(
        SaveSocialLinkRequestDto request,
        CancellationToken cancellationToken = default);

    Task<SocialLinkDto> UpdateAsync(
        int id,
        SaveSocialLinkRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
