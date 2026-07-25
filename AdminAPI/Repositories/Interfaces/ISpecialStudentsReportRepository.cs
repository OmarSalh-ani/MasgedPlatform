using AdminAPI.DTOs.SpecialStudentsReport;

namespace AdminAPI.Repositories.Interfaces;

public interface ISpecialStudentsReportRepository
{
    Task<List<SpecialStudentsReportRowDto>> GetSpecialStudentsAsync(
        CancellationToken cancellationToken = default);
}
