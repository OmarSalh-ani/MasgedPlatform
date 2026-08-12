using System.Globalization;
using AdminAPI.Data;
using AdminAPI.DTOs.CircleVisitRating;
using AdminAPI.DTOs.Common;
using AdminAPI.Exceptions;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class CircleVisitRatingService(
    ICircleVisitRatingRepository repository,
    ICurrentUserContext currentUser,
    AdminDbContext db,
    IOptions<TeacherUploadOptions> uploadOptions) : ICircleVisitRatingService
{
    public const int DefaultPageSize = 15;

    public async Task<PagedResultDto<CircleVisitRatingListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? DefaultPageSize : pageSize;
        var (items, totalCount) = await repository.GetPagedAsync(
            page, size, currentUser.IsAdmin, currentUser.TeacherId, cancellationToken);

        return new PagedResultDto<CircleVisitRatingListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size,
            TotalPages = (int)Math.Ceiling(totalCount / (double)size),
        };
    }

    public async Task<CircleVisitRatingDetailDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetScopedEntityAsync(id, cancellationToken);
        return await MapDetailAsync(entity, cancellationToken);
    }

    public Task<List<CircleVisitRatingTeacherOptionDto>> GetTeachersAsync(
        CancellationToken cancellationToken = default) =>
        repository.GetTeachersAsync(currentUser.IsGirlTeacher, cancellationToken);

    public async Task<List<CircleVisitRatingCircleOptionDto>> GetCirclesAsync(
        int teacherId,
        CancellationToken cancellationToken = default)
    {
        if (teacherId <= 0)
            return [];
        return await repository.GetCirclesForTeacherAsync(
            teacherId, currentUser.IsGirlTeacher, cancellationToken);
    }

    public async Task<CircleVisitRatingVisitNumberDto> GetVisitNumberAsync(
        int teacherId,
        DateTime visitDate,
        CancellationToken cancellationToken = default)
    {
        if (teacherId <= 0)
            return new CircleVisitRatingVisitNumberDto { VisitNumber = 1 };

        var count = await repository.CountVisitsForTeacherInMonthAsync(
            teacherId, visitDate.Year, visitDate.Month, cancellationToken);
        return new CircleVisitRatingVisitNumberDto { VisitNumber = count + 1 };
    }

    public async Task<CircleVisitRatingDetailDto> CreateAsync(
        CreateCircleVisitRatingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.CanModify)
            throw new ForbiddenException("ليس لديك صلاحية لإضافة تقييم");

        var belongs = await repository.CircleBelongsToTeacherAsync(
            request.QuranCircleId, request.TeacherId, cancellationToken);
        if (!belongs)
            throw new ArgumentException("الحلقة لا تتبع المعلم المحدد");

        var visitDate = request.VisitDate.Date;
        if (!TimeSpan.TryParse(request.VisitTime, CultureInfo.InvariantCulture, out var visitTime))
            throw new ArgumentException("وقت الزيارة غير صالح");

        var visitNumber = await repository.CountVisitsForTeacherInMonthAsync(
            request.TeacherId, visitDate.Year, visitDate.Month, cancellationToken) + 1;

        var entity = new CircleVisitRating
        {
            TeacherId = request.TeacherId,
            QuranCircleId = request.QuranCircleId,
            VisitDate = visitDate,
            VisitTime = visitTime,
            VisitNumberInMonth = visitNumber,
            CreatedBy = currentUser.TeacherId,
            CreatedAt = KuwaitTime.Now,
            Items = request.Items
                .OrderBy(i => i.Sequence)
                .Select(i => new CircleVisitRatingItem
                {
                    Sequence = i.Sequence,
                    Criterion = i.Criterion.Trim(),
                    Rating = i.Rating.Trim(),
                    Notes = string.IsNullOrWhiteSpace(i.Notes) ? null : i.Notes.Trim(),
                })
                .ToList(),
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var saved = await repository.GetByIdWithItemsAsync(entity.Id, cancellationToken)
            ?? throw new KeyNotFoundException("التقييم غير موجود");
        return await MapDetailAsync(saved, cancellationToken);
    }

    public async Task<(byte[] Bytes, string ContentType, string FileName)> ExportPdfAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetScopedEntityAsync(id, cancellationToken);
        var detail = await MapDetailAsync(entity, cancellationToken);
        var mosqueName = await MasgedBrandingHelper.GetMasgedNameAsync(db, cancellationToken);
        var logoBytes = await CircleVisitRatingAssets.TryReadLogoBytesAsync(
            db, uploadOptions.Value.Directory, cancellationToken);

        var bytes = CircleVisitRatingPdfExporter.Build(detail, mosqueName, logoBytes);
        var fileName = $"تقييم_حلقة_{detail.Id}_{KuwaitTime.Now:yyyyMMddHHmmss}.pdf";
        return (bytes, "application/pdf", fileName);
    }

    private async Task<CircleVisitRating> GetScopedEntityAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdWithItemsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("التقييم غير موجود");

        if (!currentUser.IsAdmin && entity.CreatedBy != currentUser.TeacherId)
            throw new ForbiddenException("غير مصرح بعرض هذا التقييم");

        return entity;
    }

    private async Task<CircleVisitRatingDetailDto> MapDetailAsync(
        CircleVisitRating entity,
        CancellationToken cancellationToken)
    {
        var createdByName = await repository.GetTeacherNameAsync(entity.CreatedBy, cancellationToken) ?? "";
        return new CircleVisitRatingDetailDto
        {
            Id = entity.Id,
            TeacherId = entity.TeacherId,
            TeacherName = entity.Teacher?.Name ?? "",
            QuranCircleId = entity.QuranCircleId,
            CircleName = entity.QuranCircle?.Name ?? "",
            VisitDate = entity.VisitDate,
            VisitTime = entity.VisitTime.ToString(@"hh\:mm"),
            VisitNumberInMonth = entity.VisitNumberInMonth,
            CreatedByName = createdByName,
            CreatedAt = entity.CreatedAt,
            Items = entity.Items
                .OrderBy(i => i.Sequence)
                .Select(i => new CircleVisitRatingItemDto
                {
                    Sequence = i.Sequence,
                    Criterion = i.Criterion,
                    Rating = i.Rating,
                    Notes = i.Notes,
                })
                .ToList(),
        };
    }
}
