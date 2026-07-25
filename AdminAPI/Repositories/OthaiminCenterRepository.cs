using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class OthaiminCenterRepository(Data.AdminDbContext db) : IOthaiminCenterRepository
{
    public async Task<HomeFilterOptionsDto> GetFilterOptionsAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default)
    {
        var circles = await db.QuranCircles
            .AsNoTracking()
            .Where(x => x.ForGirls == isGirlTeacher)
            .OrderBy(x => x.Name)
            .Select(x => new HomeLookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);

        var teachers = await db.Teachers
            .AsNoTracking()
            .Where(x => x.IsGirlTeacher == isGirlTeacher)
            .OrderBy(x => x.Name)
            .Select(x => new HomeLookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);

        var womanActivityTypes = await db.WomanActivities
            .AsNoTracking()
            .Where(x => x.ForGirl == isGirlTeacher)
            .OrderBy(x => x.Name)
            .Select(x => new HomeLookupDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);

        return new HomeFilterOptionsDto
        {
            Circles = circles,
            TransferCircles = circles,
            Teachers = teachers,
            WomanActivityTypes = womanActivityTypes,
        };
    }

    public Task<List<int>> GetTeacherCircleIdsAsync(int teacherId, CancellationToken cancellationToken = default) =>
        db.QuranCircles
            .AsNoTracking()
            .Where(c => c.TeacherId == teacherId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

    public Task<string?> GetCircleNameAsync(int circleId, CancellationToken cancellationToken = default) =>
        db.QuranCircles
            .AsNoTracking()
            .Where(x => x.Id == circleId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResultDto<HomeStudentNameLookupDto>> GetStudentNamesAsync(
        bool isGirlTeacher,
        bool isAdmin,
        int teacherId,
        HomeStudentNameLookupFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var gender = isGirlTeacher ? "أنثى" : "ذكر";
        var query = db.MrkzStudents.AsNoTracking().Where(x => x.StudentGender == gender);

        if (!isAdmin)
        {
            var circleIds = db.QuranCircles
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.Id);
            query = query.Where(x => x.QuranCircleId != null && circleIds.Contains(x.QuranCircleId.Value));
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var term = filters.Search.Trim();
            query = query.Where(x =>
                (x.FullName != null && x.FullName.Contains(term)) ||
                x.StudentName.Contains(term));
        }

        var namesQuery = query
            .Select(x => x.FullName != null && x.FullName != string.Empty ? x.FullName : x.StudentName)
            .Where(name => name != null && name != string.Empty)
            .Distinct();

        var page = filters.PageNumber < 1 ? 1 : filters.PageNumber;
        var size = filters.PageSize < 1 ? 20 : filters.PageSize;
        var totalCount = await namesQuery.CountAsync(cancellationToken);
        var items = await namesQuery
            .OrderBy(name => name)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(name => new HomeStudentNameLookupDto { Name = name })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<HomeStudentNameLookupDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)size),
        };
    }
}
