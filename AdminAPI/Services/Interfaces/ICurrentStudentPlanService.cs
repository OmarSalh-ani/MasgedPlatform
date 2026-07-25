using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.CurrentStudentsPlans;

namespace AdminAPI.Services.Interfaces;

public interface ICurrentStudentPlanService
{
    Task<PagedResultDto<CurrentStudentPlanListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        int? studentId = null,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<CurrentStudentPlanStudentLookupDto>> GetStudentsAsync(
        CurrentStudentPlanStudentLookupFiltersDto filters,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
