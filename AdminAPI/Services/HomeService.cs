using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public partial class HomeService(
    Data.AdminDbContext db,
    IHomeRepository repository,
    ICurrentUserContext currentUser,
    IOptions<PublicSiteOptions> publicSiteOptions,
    StudentQrTokenService qrTokenService) : IHomeService
{
    public async Task<PagedResultDto<HomeStudentListItemDto>> GetListAsync(
        HomeListFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var teacherCircleIds = await GetTeacherCircleIdsAsync(cancellationToken);
        var query = HomeStudentQueryBuilder.Build(db, currentUser, filters, teacherCircleIds);
        var baseUrl = publicSiteOptions.Value.BaseUrl.TrimEnd('/');

        var projected = query.Select(x => new HomeStudentListItemDto
        {
            Id = x.Id,
            StudentName = x.FullName != null && x.FullName != string.Empty ? x.FullName : x.StudentName,
            FatherName = x.FatherName,
            FatherPhone = x.FatherPhone,
            FatherPhone2 = x.FatherPhone2,
            StudentPhone = x.StudentPhone,
            StudentGender = x.StudentGender,
            Age = x.Age,
            Birthdate = x.Birthdate.HasValue
                ? x.Birthdate.Value.ToString("dd/MM/yyyy")
                : string.Empty,
            CreatedAt = x.CreatedAt.HasValue
                ? x.CreatedAt.Value.ToString("dd/MM/yyyy")
                : string.Empty,
            CircleName = x.QuranCircle != null ? x.QuranCircle.Name : string.Empty,
            QuranCircleId = x.QuranCircleId,
            LeaveCount = x.CircleAttendances.Count(c => c.StudentId == x.Id && !c.IsHere),
            WomanActivityType = x.WomanActivity != null ? x.WomanActivity.Name : string.Empty,
            LearnCertificate = x.LearnCertificate,
            CompleteFollowup = x.ParentFollowup != null ? "نعم" : "لا",
            IsSpecial = x.IsSpecial ? "نعم" : "لا",
            IsElite = x.IsElite ? "نعم" : "لا",
            StudentImage = x.ParentFollowup != null && x.ParentFollowup.PhotoPath != null
                ? baseUrl + x.ParentFollowup.PhotoPath.Replace("~", string.Empty)
                : string.Empty,
            PlanLevelName = x.PlanLevel != null ? x.PlanLevel.LevelName : "غير محدد",
        });

        var page = filters.PageNumber < 1 ? 1 : filters.PageNumber;
        var size = filters.PageSize < 1 ? 20 : filters.PageSize;
        var totalCount = await projected.CountAsync(cancellationToken);
        var items = await projected.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return new PagedResultDto<HomeStudentListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)size),
        };
    }

    public Task<HomeFilterOptionsDto> GetFilterOptionsAsync(CancellationToken cancellationToken = default) =>
        repository.GetFilterOptionsAsync(currentUser.IsGirlTeacher, cancellationToken);

    public Task<PagedResultDto<HomeStudentNameLookupDto>> GetStudentNamesAsync(
        HomeStudentNameLookupFiltersDto filters,
        CancellationToken cancellationToken = default) =>
        repository.GetStudentNamesAsync(
            currentUser.IsGirlTeacher,
            currentUser.IsAdmin,
            currentUser.TeacherId,
            filters,
            cancellationToken);

    public Task<string?> GetPageTitleCircleNameAsync(int circleId, CancellationToken cancellationToken = default) =>
        repository.GetCircleNameAsync(circleId, cancellationToken);

    private async Task<List<int>> GetTeacherCircleIdsAsync(CancellationToken cancellationToken)
    {
        if (currentUser.IsAdmin)
            return [];

        return await repository.GetTeacherCircleIdsAsync(currentUser.TeacherId, cancellationToken);
    }
}
