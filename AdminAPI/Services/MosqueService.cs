using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Mosques;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class MosqueService(
    IMosqueRepository repository,
    IMapper mapper,
    IOptions<MosqueUploadOptions> uploadOptions) : IMosqueService
{
    public async Task<PagedResultDto<MosqueListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        var mapped = mapper.Map<List<MosqueListItemDto>>(items);
        foreach (var item in mapped)
            item.ImageUrl = MosqueImageStorage.NormalizeImageUrl(item.ImageUrl);

        return new PagedResultDto<MosqueListItemDto>
        {
            Items = mapped,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<MosqueDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var mosque = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("المسجد غير موجود");

        return MapToDto(mosque);
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default) =>
        repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<MosqueDto> CreateAsync(
        SaveMosqueRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var imageUrl = await MosqueImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        var mosque = new Mosque
        {
            Name = request.Name.Trim(),
            Description = NormalizeOptional(request.Description),
            GoogleMapsUrl = NormalizeOptional(request.GoogleMapsUrl),
            ImageUrl = imageUrl,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.Now
        };

        await repository.AddAsync(mosque, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToDto(mosque);
    }

    public async Task<MosqueDto> UpdateAsync(
        int id,
        SaveMosqueRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var mosque = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("المسجد غير موجود");

        var imageUrl = await MosqueImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        mosque.Name = request.Name.Trim();
        mosque.Description = NormalizeOptional(request.Description);
        mosque.GoogleMapsUrl = NormalizeOptional(request.GoogleMapsUrl);
        mosque.SortOrder = request.SortOrder;
        if (imageUrl is not null)
            mosque.ImageUrl = imageUrl;

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(mosque);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private MosqueDto MapToDto(Mosque mosque)
    {
        var dto = mapper.Map<MosqueDto>(mosque);
        dto.ImageUrl = MosqueImageStorage.NormalizeImageUrl(dto.ImageUrl);
        return dto;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
