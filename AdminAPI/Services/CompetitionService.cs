using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Competitions;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class CompetitionService(
    ICompetitionRepository repository,
    IMapper mapper,
    IOptions<CompetitionUploadOptions> uploadOptions) : ICompetitionService
{
    public async Task<PagedResultDto<CompetitionListItemDto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        return new PagedResultDto<CompetitionListItemDto>
        {
            Items = mapper.Map<List<CompetitionListItemDto>>(items),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<CompetitionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default) =>
        repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<CompetitionDto> CreateAsync(
        SaveCompetitionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var imageUrl = await CompetitionImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        var entity = new Competition
        {
            Title = request.Title.Trim(),
            Description = NormalizeOptional(request.Description),
            ImageUrl = imageUrl,
            LinkUrl = NormalizeOptional(request.LinkUrl),
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.Now
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<CompetitionDto> UpdateAsync(
        int id,
        SaveCompetitionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("المسابقة غير موجودة");

        var imageUrl = await CompetitionImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        entity.Title = request.Title.Trim();
        entity.Description = NormalizeOptional(request.Description);
        entity.LinkUrl = NormalizeOptional(request.LinkUrl);
        entity.SortOrder = request.SortOrder;
        if (imageUrl is not null)
            entity.ImageUrl = imageUrl;

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
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
            CompetitionImageStorage.DeleteIfExists(entity.ImageUrl, uploadOptions.Value.Directory);

        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private CompetitionDto MapToDto(Competition entity)
    {
        var dto = mapper.Map<CompetitionDto>(entity);
        dto.ImageUrl = CompetitionImageStorage.NormalizeImageUrl(dto.ImageUrl);
        return dto;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
