using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using TeacherMemorizingCard = MasgedTeacherMobileAPI.Entities.StudentMemorizingCard;

namespace MasgedParentMobileAPI.Services;

public sealed class MemorizingArchiveService
{
    private const int MaxPageSize = 50;

    public async Task<PagedResultDto<MemorizingArchiveItemDto>> GetForParentAsync(
        NewMasgedTeacherAPIDBContext db,
        int studentId,
        string? surahSearch,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = db.StudentMemorizingCards
            .AsNoTracking()
            .Where(x => x.StudentId == studentId);

        if (!string.IsNullOrWhiteSpace(surahSearch))
        {
            var term = surahSearch.Trim();
            query = query.Where(x => x.TestFrom.Contains(term));
        }

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

        if (!string.IsNullOrWhiteSpace(surahSearch))
        {
            var term = surahSearch.Trim();
            query = query.Where(x => x.TestFrom.Contains(term));
        }

        return await PageTeacherAsync(
            query.OrderByDescending(x => x.CreatedAt),
            page,
            pageSize,
            cancellationToken);
    }

    private static async Task<PagedResultDto<MemorizingArchiveItemDto>> PageParentAsync(
        IQueryable<StudentMemorizingCard> query,
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
