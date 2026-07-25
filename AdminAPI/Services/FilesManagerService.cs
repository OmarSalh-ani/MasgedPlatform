using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.FilesManager;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class FilesManagerService(
    IFilesManagerRepository repository,
    IMapper mapper,
    IOptions<FilesManagerUploadOptions> uploadOptions) : IFilesManagerService
{
    public async Task<PagedResultDto<FilesManagerListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;
        var dtos = items.Select(MapToListItem).ToList();

        return new PagedResultDto<FilesManagerListItemDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<FilesManagerDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الملف غير موجود");

        return MapToDto(entity);
    }

    public async Task<FilesManagerDto> CreateAsync(
        SaveFilesManagerRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var savedPath = await SaveUploadedFileAsync(request.File, cancellationToken);

        var entity = new FilesManager
        {
            Name = request.Name.Trim(),
            FilePath = savedPath
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<FilesManagerDto> UpdateAsync(
        int id,
        SaveFilesManagerRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الملف غير موجود");

        var savedPath = await SaveUploadedFileAsync(request.File, cancellationToken);
        FilesManagerStorage.DeleteIfExists(entity.FilePath, uploadOptions.Value.Directory);

        entity.Name = request.Name.Trim();
        entity.FilePath = savedPath;
        await repository.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default)
    {
        var (items, _) = await repository.GetPagedAsync(1, 0, cancellationToken);
        var dtos = items.Select(MapToListItem).ToList();
        return FilesManagerExcelExporter.Export(dtos);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return false;

        FilesManagerStorage.DeleteIfExists(entity.FilePath, uploadOptions.Value.Directory);

        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<string> SaveUploadedFileAsync(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            throw new ValidationException([
                new ValidationFailure("File", "يرجى اختيار ملف للرفع")
            ]);
        }

        var savedPath = await FilesManagerStorage.SaveAsync(
            file,
            uploadOptions.Value.Directory,
            cancellationToken);

        if (string.IsNullOrEmpty(savedPath))
        {
            throw new ValidationException([
                new ValidationFailure("File", "يرجى اختيار ملف للرفع")
            ]);
        }

        return savedPath;
    }

    private FilesManagerDto MapToDto(FilesManager entity)
    {
        var dto = mapper.Map<FilesManagerDto>(entity);
        dto.FileUrl = FilesManagerStorage.BuildPublicUrl(entity.FilePath, uploadOptions.Value.PublicBaseUrl);
        return dto;
    }

    private FilesManagerListItemDto MapToListItem(FilesManager entity)
    {
        var dto = mapper.Map<FilesManagerListItemDto>(entity);
        dto.FileUrl = FilesManagerStorage.BuildPublicUrl(entity.FilePath, uploadOptions.Value.PublicBaseUrl);
        return dto;
    }
}
