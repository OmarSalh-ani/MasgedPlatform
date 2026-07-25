using AdminAPI.DTOs.SpecialStudentsReport;

namespace AdminAPI.Services.Interfaces;

public interface ISpecialStudentsReportService
{
    Task<SpecialStudentsReportResponseDto> GetReportAsync(CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string FileName, int StudentsCount, int CirclesCount)?> ExportAsync(
        CancellationToken cancellationToken = default);
}
