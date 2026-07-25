using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;

namespace AdminAPI.Repositories.Interfaces;

public interface IOthaiminCenterRepository
{
    Task<HomeFilterOptionsDto> GetFilterOptionsAsync(bool isGirlTeacher, CancellationToken cancellationToken = default);

    Task<List<int>> GetTeacherCircleIdsAsync(int teacherId, CancellationToken cancellationToken = default);

    Task<string?> GetCircleNameAsync(int circleId, CancellationToken cancellationToken = default);

    Task<PagedResultDto<HomeStudentNameLookupDto>> GetStudentNamesAsync(
        bool isGirlTeacher,
        bool isAdmin,
        int teacherId,
        HomeStudentNameLookupFiltersDto filters,
        CancellationToken cancellationToken = default);
}
