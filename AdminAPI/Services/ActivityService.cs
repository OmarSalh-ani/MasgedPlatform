using AdminAPI.DTOs.Activities;
using AdminAPI.DTOs.Activity;
using AdminAPI.DTOs.Common;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class ActivityService(
    IActivityRepository repository,
    IMapper mapper,
    IOptions<ActivityUploadOptions> uploadOptions) : IActivityService
{
    public async Task<PagedResultDto<ActivityListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        var mapped = mapper.Map<List<ActivityListItemDto>>(items);
        foreach (var item in mapped)
            item.ImageUrl = ActivityImageStorage.NormalizeImageUrl(item.ImageUrl);

        return new PagedResultDto<ActivityListItemDto>
        {
            Items = mapped,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<ActivityDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var activity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("النشاط غير موجود");

        return MapToDto(activity);
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default) =>
        repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<ActivityDto> CreateAsync(
        SaveActivityRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var imageUrl = await ActivityImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        var activity = new Activity
        {
            Title = request.Title.Trim(),
            Description = NormalizeOptional(request.Description),
            SortOrder = request.SortOrder,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.Now
        };

        await repository.AddAsync(activity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToDto(activity);
    }

    public async Task<ActivityDto> UpdateAsync(
        int id,
        SaveActivityRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var activity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("النشاط غير موجود");

        var imageUrl = await ActivityImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        activity.Title = request.Title.Trim();
        activity.Description = NormalizeOptional(request.Description);
        activity.SortOrder = request.SortOrder;
        if (imageUrl is not null)
            activity.ImageUrl = imageUrl;

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(activity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private ActivityDto MapToDto(Activity activity)
    {
        var dto = mapper.Map<ActivityDto>(activity);
        dto.ImageUrl = ActivityImageStorage.NormalizeImageUrl(dto.ImageUrl);
        return dto;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
