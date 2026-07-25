using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.WomansActivities;
using AdminAPI.Exceptions;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class WomansActivityService(
    IWomansActivityRepository repository,
    ICurrentUserContext currentUser) : IWomansActivityService
{
    public async Task<PagedResultDto<WomanActivityListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var items = await repository.GetListAsync(currentUser.IsGirlTeacher, cancellationToken);
        return ToPagedResult(items, pageNumber, pageSize);
    }

    public async Task<WomanActivityDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, currentUser.IsGirlTeacher, cancellationToken)
            ?? throw new KeyNotFoundException("النشاط غير موجود");
        return MapToDto(entity);
    }

    public async Task<WomanActivityDto> CreateAsync(
        SaveWomanActivityRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        var entity = new WomanActivity
        {
            Name = request.Name.Trim(),
            IsVisible = request.IsVisible,
            ForGirl = currentUser.IsGirlTeacher,
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<WomanActivityDto> UpdateAsync(
        int id,
        SaveWomanActivityRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        var entity = await repository.GetByIdAsync(id, currentUser.IsGirlTeacher, cancellationToken)
            ?? throw new KeyNotFoundException("النشاط غير موجود");

        entity.Name = request.Name.Trim();
        entity.IsVisible = request.IsVisible;
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!currentUser.CanModify)
            return false;

        var entity = await repository.GetByIdAsync(id, currentUser.IsGirlTeacher, cancellationToken);
        if (entity is null)
            return false;

        entity.IsVisible = false;
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetListAsync(currentUser.IsGirlTeacher, cancellationToken);
        return WomansActivityExcelExporter.Build(items);
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new ForbiddenException("ليس لديك صلاحية لتعديل النشاطات");
    }

    private static WomanActivityDto MapToDto(WomanActivity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        IsVisible = entity.IsVisible,
        ForGirl = entity.ForGirl,
    };

    private static PagedResultDto<WomanActivityListItemDto> ToPagedResult(
        List<WomanActivityListItemDto> items,
        int pageNumber,
        int pageSize)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var totalCount = items.Count;
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;
        var pagedItems = pageSize <= 0
            ? items
            : items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResultDto<WomanActivityListItemDto>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize),
        };
    }
}
