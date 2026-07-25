using AdminAPI.DTOs.Expensives;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public partial class ExpensiveService(
    IExpensiveRepository repository,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IOptions<ExpensiveUploadOptions> uploadOptions) : IExpensiveService
{
    public async Task<ExpensiveDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetScopedEntityAsync(id, cancellationToken);
        return MapToDto(entity);
    }

    public async Task<ExpensiveDto> CreateAsync(
        SaveExpensiveRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        var folderPath = await SaveUploadedFilesAsync(null, request.Files, cancellationToken);
        var entity = new Expensive
        {
            Reason = request.Reason.Trim(),
            Supplier = request.Supplier.Trim(),
            Notes = NormalizeOptional(request.Notes),
            TotalAmount = request.TotalAmount,
            TeacherId = currentUser.TeacherId,
            ForGirls = currentUser.IsGirlTeacher,
            CreatedAt = KuwaitTime.Now,
            AttachmentsFolder = folderPath
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<ExpensiveDto> UpdateAsync(
        int id,
        SaveExpensiveRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        var entity = await GetScopedEntityAsync(id, cancellationToken);
        var folderPath = await SaveUploadedFilesAsync(entity.AttachmentsFolder, request.Files, cancellationToken);

        entity.Reason = request.Reason.Trim();
        entity.Supplier = request.Supplier.Trim();
        entity.Notes = NormalizeOptional(request.Notes);
        entity.TotalAmount = request.TotalAmount;
        entity.TeacherId = currentUser.TeacherId;
        entity.ForGirls = currentUser.IsGirlTeacher;
        entity.AttachmentsFolder = folderPath;

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task DeleteAttachmentAsync(
        int id,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لتعديل المصروفات");

        var entity = await GetScopedEntityAsync(id, cancellationToken);
        ExpensiveAttachmentStorage.DeleteFile(entity.AttachmentsFolder, fileName);

        if (string.IsNullOrWhiteSpace(entity.AttachmentsFolder)
            || !Directory.Exists(entity.AttachmentsFolder)
            || !Directory.EnumerateFileSystemEntries(entity.AttachmentsFolder).Any())
        {
            entity.AttachmentsFolder = string.Empty;
            await repository.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<(string Path, string FileName)> GetAttachmentFileAsync(
        int id,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetScopedEntityAsync(id, cancellationToken);
        var path = ExpensiveAttachmentStorage.ResolveFilePath(entity.AttachmentsFolder, fileName)
            ?? throw new KeyNotFoundException("الملف غير موجود");

        return (path, Path.GetFileName(path));
    }

    private async Task<Expensive> GetScopedEntityAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("المصروف غير موجود");

        if (entity.ForGirls != currentUser.IsGirlTeacher)
            throw new KeyNotFoundException("المصروف غير موجود");

        return entity;
    }

    private async Task<string> SaveUploadedFilesAsync(
        string? existingFolder,
        List<IFormFile>? files,
        CancellationToken cancellationToken)
    {
        var folderPath = existingFolder ?? string.Empty;
        if (files is null || files.Count == 0)
            return folderPath;

        if (string.IsNullOrWhiteSpace(folderPath))
            folderPath = ExpensiveAttachmentStorage.CreateSubfolderPath(uploadOptions.Value.Directory);

        await ExpensiveAttachmentStorage.SaveFilesAsync(files, folderPath, cancellationToken);
        return folderPath;
    }

    private ExpensiveDto MapToDto(Expensive entity)
    {
        var dto = mapper.Map<ExpensiveDto>(entity);
        dto.Attachments = ExpensiveAttachmentStorage.ListFiles(entity.AttachmentsFolder);
        return dto;
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لحفظ أو تعديل المصروفات");
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
