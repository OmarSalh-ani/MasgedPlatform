using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.QuranCircles;
using AdminAPI.Exceptions;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using AdminAPI.Data;

namespace AdminAPI.Services;

public class QuranCircleService(
    IQuranCircleRepository repository,
    AdminDbContext db,
    ICurrentUserContext currentUser) : IQuranCircleService
{
    public async Task<PagedResultDto<QuranCircleListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        int? teacherId,
        CancellationToken cancellationToken = default)
    {
        var items = await repository.GetListAsync(
            currentUser.IsGirlTeacher,
            currentUser.IsAdmin,
            currentUser.TeacherId,
            teacherId,
            cancellationToken);
        return ToPagedResult(items, pageNumber, pageSize);
    }

    public async Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetExportListAsync(currentUser.IsGirlTeacher, cancellationToken);
        return QuranCircleExcelExporter.Build(items);
    }

    public async Task<QuranCircleDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var circle = await repository.GetByIdWithDaysAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الحلقة غير موجودة");
        return MapToDto(circle);
    }

    public async Task<List<TeacherOptionDto>> GetTeachersAsync(CancellationToken cancellationToken = default)
    {
        return await db.Teachers
            .AsNoTracking()
            .Where(t => t.IsGirlTeacher == currentUser.IsGirlTeacher)
            .OrderBy(t => t.Name)
            .Select(t => new TeacherOptionDto { Id = t.Id, Name = t.Name })
            .ToListAsync(cancellationToken);
    }

    public async Task<QuranCircleDto> CreateAsync(
        SaveQuranCircleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var circle = new QuranCircle
        {
            Name = request.Name.Trim(),
            TeacherId = request.TeacherId,
            ForGirls = request.ForGirls,
            CreatedAt = KuwaitTime.Now,
            CreatedBy = currentUser.TeacherId,
        };

        await repository.AddAsync(circle, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var created = await repository.GetByIdWithDaysAsync(circle.Id, cancellationToken)
            ?? throw new KeyNotFoundException("الحلقة غير موجودة");
        return MapToDto(created);
    }

    public async Task<QuranCircleDto> UpdateAsync(
        int id,
        SaveQuranCircleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var circle = await repository.GetByIdWithDaysAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الحلقة غير موجودة");

        circle.Name = request.Name.Trim();
        circle.TeacherId = request.TeacherId;
        circle.ForGirls = request.ForGirls;

        await repository.SaveChangesAsync(cancellationToken);

        var updated = await repository.GetByIdWithDaysAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الحلقة غير موجودة");
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var circle = await repository.GetByIdWithDaysAsync(id, cancellationToken);
        if (circle is null)
            return false;

        var teacherId = circle.TeacherId;
        var deleted = await repository.DeleteWithRelatedAsync(id, teacherId, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new ForbiddenException("ليس لديك صلاحية لتعديل أو إضافة حلقات");
    }

    private static PagedResultDto<QuranCircleListItemDto> ToPagedResult(
        List<QuranCircleListItemDto> items,
        int pageNumber,
        int pageSize)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var totalCount = items.Count;
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;
        var pagedItems = pageSize <= 0
            ? items
            : items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResultDto<QuranCircleListItemDto>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static QuranCircleDto MapToDto(QuranCircle circle) => new()
    {
        Id = circle.Id,
        Name = circle.Name,
        TeacherId = circle.TeacherId,
        ForGirls = circle.ForGirls ?? false,
    };
}
