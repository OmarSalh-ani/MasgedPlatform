using AdminAPI.Data;
using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Teachers;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class TeacherService(
    AdminDbContext db,
    ITeacherRepository repository,
    ICurrentUserContext currentUser,
    IOptions<PublicSiteOptions> publicSiteOptions,
    IOptions<TeacherUploadOptions> uploadOptions) : ITeacherService
{
    public async Task<PagedResultDto<TeacherListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var items = await repository.GetListAsync(currentUser.IsGirlTeacher, cancellationToken);
        MapImageUrls(items);

        var page = pageNumber < 1 ? 1 : pageNumber;
        var totalCount = items.Count;
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;
        var pagedItems = pageSize <= 0
            ? items
            : items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResultDto<TeacherListItemDto>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize),
        };
    }

    public async Task<TeacherCardPrintDto> GetCardPrintAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin)
            throw new UnauthorizedAccessException("غير مصرح");

        var teacher = await db.Teachers.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new { x.Id, x.Name, x.Image })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("المشرف غير موجود");

        return new TeacherCardPrintDto
        {
            Id = teacher.Id,
            Name = teacher.Name,
            ImageUrl = TeacherImageStorage.BuildPublicImageUrl(
                teacher.Image,
                publicSiteOptions.Value.BaseUrl),
        };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var imageFileName = await repository.DeleteWithRelatedAsync(
            id,
            currentUser.IsGirlTeacher,
            restrictCirclesToForGirls: true,
            cancellationToken);
        if (imageFileName is null)
            return false;

        TeacherImageStorage.DeleteIfExists(imageFileName, uploadOptions.Value.Directory);
        return true;
    }

    public async Task<byte[]> ExportToExcelAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetExportListAsync(currentUser.IsGirlTeacher, cancellationToken);
        return TeacherExcelExporter.Build(items);
    }

    private void MapImageUrls(List<TeacherListItemDto> items)
    {
        var baseUrl = publicSiteOptions.Value.BaseUrl;
        foreach (var item in items)
        {
            item.ImageUrl = TeacherImageStorage.BuildPublicImageUrl(item.ImageUrl, baseUrl);
        }
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لتعديل أو حذف المعلمين");
    }
}
