using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.SocialLink;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;

namespace AdminAPI.Services;

public class SocialLinkService(
    ISocialLinkRepository repository,
    IMapper mapper) : ISocialLinkService
{
    public async Task<PagedResultDto<SocialLinkListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        return new PagedResultDto<SocialLinkListItemDto>
        {
            Items = mapper.Map<List<SocialLinkListItemDto>>(items),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<SocialLinkDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var link = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الرابط غير موجود");

        return mapper.Map<SocialLinkDto>(link);
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default) =>
        repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<SocialLinkDto> CreateAsync(
        SaveSocialLinkRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var link = new SocialLink
        {
            PlatformName = request.PlatformName.Trim(),
            Url = request.Url.Trim(),
            IconClass = NormalizeOptional(request.IconClass),
            SortOrder = request.SortOrder
        };

        await repository.AddAsync(link, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<SocialLinkDto>(link);
    }

    public async Task<SocialLinkDto> UpdateAsync(
        int id,
        SaveSocialLinkRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var link = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الرابط غير موجود");

        link.PlatformName = request.PlatformName.Trim();
        link.Url = request.Url.Trim();
        link.IconClass = NormalizeOptional(request.IconClass);
        link.SortOrder = request.SortOrder;

        await repository.SaveChangesAsync(cancellationToken);
        return mapper.Map<SocialLinkDto>(link);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
