using System.Globalization;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using ParentMemorizingCard = MasgedParentMobileAPI.Models.StudentMemorizingCard;
using TeacherMemorizingCard = MasgedTeacherMobileAPI.Entities.StudentMemorizingCard;

namespace MasgedParentMobileAPI.Services;

public sealed class MemorizingArchiveService
{
    private const int MaxPageSize = 50;
    private const string TypeRevision = "مراجعة";
    private const string UnitJozz = "جزء";
    private const string UnitHezb = "حزب";

    public async Task<PagedResultDto<MemorizingArchiveItemDto>> GetForParentAsync(
        NewMasgedTeacherAPIDBContext db,
        int studentId,
        string? surahSearch,
        string? typeFilter,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = db.StudentMemorizingCards
            .AsNoTracking()
            .Where(x => x.StudentId == studentId);

        query = ApplyFilters(query, surahSearch, typeFilter);

        return await PageParentAsync(
            query.OrderByDescending(x => x.CreatedAt),
            page,
            pageSize,
            cancellationToken);
    }

    public async Task<PagedResultDto<MemorizingArchiveItemDto>> GetForTeacherAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        string? surahSearch,
        string? typeFilter,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        if (!await StudentCircleAccessHelper.CanReadStudentAsync(
                db, studentId, circleId, cancellationToken))
        {
            return new PagedResultDto<MemorizingArchiveItemDto>
            {
                Page = page,
                PageSize = pageSize,
            };
        }

        var isCurrentMember = await StudentCircleAccessHelper.IsCurrentMemberAsync(
            db, studentId, circleId, cancellationToken);

        var query = db.StudentMemorizingCards
            .AsNoTracking()
            .Where(x => x.StudentId == studentId);

        if (!isCurrentMember)
            query = query.Where(x => x.CircleId == circleId);

        query = ApplyTeacherFilters(query, surahSearch, typeFilter);

        return await PageTeacherAsync(
            query.OrderByDescending(x => x.CreatedAt),
            page,
            pageSize,
            cancellationToken);
    }

    public async Task<(MemorizingArchiveItemDto? Item, string? Error)> CreateJuzHizbReviewAsync(
        AppDbContext db,
        int studentId,
        int teacherId,
        int circleId,
        CreateJuzHizbReviewDto request,
        CancellationToken cancellationToken)
    {
        if (!await StudentCircleAccessHelper.CanWriteStudentAsync(
                db, studentId, circleId, cancellationToken))
            return (null, "الطالب غير موجود");

        var unitType = request.UnitType?.Trim() ?? string.Empty;
        if (unitType is not (UnitJozz or UnitHezb))
            return (null, "نوع الوحدة غير صالح. اختر جزء أو حزب");

        var maxNumber = unitType == UnitJozz ? 30 : 60;
        if (request.Number < 1 || request.Number > maxNumber)
            return (null, $"رقم {unitType} يجب أن يكون بين 1 و {maxNumber}");

        var now = KuwaitTime.Now;
        var numberText = request.Number.ToString(CultureInfo.InvariantCulture);
        var card = new TeacherMemorizingCard
        {
            CreatedAt = now,
            CircleId = circleId,
            DayName = now.ToString("dddd", CultureInfo.CurrentCulture),
            IsDone = "نعم",
            TeacherId = teacherId,
            StudentId = studentId,
            TestFrom = numberText,
            TestTo = numberText,
            SurahName = unitType,
            TheType = TypeRevision,
            IsSaveDone = "لا",
        };

        db.StudentMemorizingCards.Add(card);
        await db.SaveChangesAsync(cancellationToken);

        return (MapItem(card), null);
    }

    private static IQueryable<ParentMemorizingCard> ApplyFilters(
        IQueryable<ParentMemorizingCard> query,
        string? surahSearch,
        string? typeFilter)
    {
        var normalizedType = NormalizeTypeFilter(typeFilter);
        if (normalizedType is not null)
            query = query.Where(x => x.TheType == normalizedType);

        if (!string.IsNullOrWhiteSpace(surahSearch))
        {
            var term = surahSearch.Trim();
            query = query.Where(x =>
                (x.SurahName != null && x.SurahName.Contains(term)) ||
                x.TestFrom.Contains(term));
        }

        return query;
    }

    private static IQueryable<TeacherMemorizingCard> ApplyTeacherFilters(
        IQueryable<TeacherMemorizingCard> query,
        string? surahSearch,
        string? typeFilter)
    {
        var normalizedType = NormalizeTypeFilter(typeFilter);
        if (normalizedType is not null)
            query = query.Where(x => x.TheType == normalizedType);

        if (!string.IsNullOrWhiteSpace(surahSearch))
        {
            var term = surahSearch.Trim();
            query = query.Where(x =>
                (x.SurahName != null && x.SurahName.Contains(term)) ||
                x.TestFrom.Contains(term));
        }

        return query;
    }

    private static string? NormalizeTypeFilter(string? typeFilter)
    {
        var trimmed = typeFilter?.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return null;

        return trimmed is "حفظ" or "مراجعة" ? trimmed : null;
    }

    private static MemorizingArchiveItemDto MapItem(TeacherMemorizingCard card) =>
        new()
        {
            Id = card.Id,
            TheType = card.TheType,
            TestFrom = card.TestFrom,
            TestTo = card.TestTo,
            SurahName = card.SurahName ?? string.Empty,
            IsDone = card.IsDone,
            Notes = card.Notes,
            CreatedAt = card.CreatedAt,
        };

    private static async Task<PagedResultDto<MemorizingArchiveItemDto>> PageParentAsync(
        IQueryable<ParentMemorizingCard> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var skip = (page - 1) * pageSize;

        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(x => new MemorizingArchiveItemDto
            {
                Id = x.Id,
                TheType = x.TheType,
                TestFrom = x.TestFrom,
                TestTo = x.TestTo,
                SurahName = x.SurahName ?? string.Empty,
                IsDone = x.IsDone,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<MemorizingArchiveItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
        };
    }

    private static async Task<PagedResultDto<MemorizingArchiveItemDto>> PageTeacherAsync(
        IQueryable<TeacherMemorizingCard> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var skip = (page - 1) * pageSize;

        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(x => new MemorizingArchiveItemDto
            {
                Id = x.Id,
                TheType = x.TheType,
                TestFrom = x.TestFrom,
                TestTo = x.TestTo,
                SurahName = x.SurahName ?? string.Empty,
                IsDone = x.IsDone,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<MemorizingArchiveItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
        };
    }
}
