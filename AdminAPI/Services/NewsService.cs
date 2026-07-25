using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.News;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class NewsService(
    INewsRepository repository,
    IMapper mapper,
    IOptions<NewsUploadOptions> uploadOptions) : INewsService
{
    public async Task<PagedResultDto<NewsListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        return new PagedResultDto<NewsListItemDto>
        {
            Items = mapper.Map<List<NewsListItemDto>>(items),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<NewsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الخبر غير موجود");

        return MapToDto(entity);
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default) =>
        repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<NewsDto> CreateAsync(
        SaveNewsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var imageUrl = await NewsImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        var entity = new NewsItem
        {
            Title = request.Title.Trim(),
            Description = NormalizeOptional(request.Description),
            NewsDate = ResolveNewsDate(request.NewsDate),
            ImageUrl = imageUrl,
            LinkUrl = null,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.Now
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<NewsDto> UpdateAsync(
        int id,
        SaveNewsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الخبر غير موجود");

        var imageUrl = await NewsImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        entity.Title = request.Title.Trim();
        entity.Description = NormalizeOptional(request.Description);
        entity.NewsDate = ResolveNewsDate(request.NewsDate);
        entity.LinkUrl = null;
        entity.SortOrder = request.SortOrder;
        if (imageUrl is not null)
            entity.ImageUrl = imageUrl;

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return false;

        NewsImageStorage.DeleteIfExists(entity.ImageUrl, uploadOptions.Value.Directory);

        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private NewsDto MapToDto(NewsItem entity)
    {
        var dto = mapper.Map<NewsDto>(entity);
        dto.ImageUrl = NewsImageStorage.NormalizeImageUrl(dto.ImageUrl);
        return dto;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTime ResolveNewsDate(DateTime newsDate) =>
        newsDate == default ? DateTime.Now : newsDate;
}
