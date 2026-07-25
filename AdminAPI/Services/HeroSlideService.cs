using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.HeroSlides;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class HeroSlideService(
    IHeroSlideRepository repository,
    IMapper mapper,
    IOptions<HeroUploadOptions> uploadOptions) : IHeroSlideService
{
    public async Task<PagedResultDto<HeroSlideListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        var dtos = mapper.Map<List<HeroSlideListItemDto>>(items);
        foreach (var dto in dtos)
            dto.ImageUrl = HeroImageStorage.NormalizeImageUrl(dto.ImageUrl);

        return new PagedResultDto<HeroSlideListItemDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<HeroSlideDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var slide = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الصورة غير موجودة");

        return MapToDto(slide);
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default) =>
        repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<HeroSlideDto> CreateAsync(
        SaveHeroSlideRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var uploadedUrls = await SaveUploadedImagesAsync(request.Images, cancellationToken);
        if (uploadedUrls.Count == 0)
            throw new ValidationException([new ValidationFailure("", "يرجى اختيار صورة أو أكثر للرفع.")]);

        var sortOrder = request.SortOrder;
        HeroSlide? first = null;

        for (var i = 0; i < uploadedUrls.Count; i++)
        {
            var entity = new HeroSlide
            {
                ImageUrl = uploadedUrls[i],
                SortOrder = sortOrder + i,
                CreatedAt = DateTime.Now
            };
            await repository.AddAsync(entity, cancellationToken);
            first ??= entity;
        }

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(first!);
    }

    public async Task<HeroSlideDto> UpdateAsync(
        int id,
        SaveHeroSlideRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var uploadedUrls = await SaveUploadedImagesAsync(request.Images, cancellationToken);

        if (uploadedUrls.Count == 0)
        {
            var existing = await repository.GetByIdAsync(id, cancellationToken);
            if (existing is not null && !string.IsNullOrEmpty(existing.ImageUrl))
                uploadedUrls.Add(existing.ImageUrl);
        }

        if (uploadedUrls.Count == 0)
            throw new ValidationException([new ValidationFailure("", "يرجى اختيار صورة أو أكثر للرفع.")]);

        var slide = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الصورة غير موجودة");

        slide.ImageUrl = uploadedUrls[0];
        slide.SortOrder = request.SortOrder;

        for (var i = 1; i < uploadedUrls.Count; i++)
        {
            await repository.AddAsync(new HeroSlide
            {
                ImageUrl = uploadedUrls[i],
                SortOrder = request.SortOrder + i,
                CreatedAt = DateTime.Now
            }, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(slide);
    }

    public async Task<bool> DeleteAsync(
        int id,
        bool deleteImageFile,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return false;

        if (deleteImageFile)
            HeroImageStorage.DeleteIfExists(entity.ImageUrl, uploadOptions.Value.Directory);

        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<List<string>> SaveUploadedImagesAsync(
        IEnumerable<IFormFile> files,
        CancellationToken cancellationToken)
    {
        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length <= 0)
                continue;

            var fileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrEmpty(fileName))
                continue;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!HeroImageStorage.AllowedExtensions.Contains(extension))
            {
                throw new ValidationException([
                    new ValidationFailure("", "يرجى اختيار ملفات صورة فقط (JPG, PNG, GIF, WebP).")
                ]);
            }

            var imageUrl = await HeroImageStorage.SaveAsync(
                file,
                uploadOptions.Value.Directory,
                cancellationToken);

            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ValidationException([
                    new ValidationFailure("", "حدث خطأ أثناء رفع الصورة: " + fileName)
                ]);
            }

            uploadedUrls.Add(imageUrl);
        }

        return uploadedUrls;
    }

    private HeroSlideDto MapToDto(HeroSlide slide)
    {
        var dto = mapper.Map<HeroSlideDto>(slide);
        dto.ImageUrl = HeroImageStorage.NormalizeImageUrl(dto.ImageUrl);
        return dto;
    }
}
