using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.DTOs.AttendanceReport;

namespace AdminAPI.Services.Interfaces;

public interface IHomeService
{
    Task<PagedResultDto<HomeStudentListItemDto>> GetListAsync(
        HomeListFiltersDto filters,
        CancellationToken cancellationToken = default);

    Task<HomeFilterOptionsDto> GetFilterOptionsAsync(CancellationToken cancellationToken = default);

    Task<PagedResultDto<HomeStudentNameLookupDto>> GetStudentNamesAsync(
        HomeStudentNameLookupFiltersDto filters,
        CancellationToken cancellationToken = default);

    Task<string?> GetPageTitleCircleNameAsync(int circleId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportExcelAsync(HomeListFiltersDto filters, CancellationToken cancellationToken = default);

    Task<string> SendWhatsappAsync(
        SendAttendanceWhatsappRequestDto request,
        string? base64Image,
        CancellationToken cancellationToken = default);

    Task<int> TransferStudentsAsync(
        TransferHomeStudentsRequestDto request,
        CancellationToken cancellationToken = default);

    Task<int> RemoveFromCircleAsync(
        RemoveHomeStudentsFromCircleRequestDto request,
        CancellationToken cancellationToken = default);

    Task<int> CreateCircleAsync(
        CreateHomeCircleRequestDto request,
        CancellationToken cancellationToken = default);

    Task DeleteStudentAsync(int studentId, CancellationToken cancellationToken = default);

    Task<List<HomeStudentTestDto>> GetStudentTestsAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<List<HomeStudentReviewDto>> GetStudentReviewsAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<HomeRegistrationSettingsDto> GetRegistrationSettingsAsync(
        CancellationToken cancellationToken = default);

    Task UpdateRegistrationSettingsAsync(
        UpdateHomeRegistrationRequestDto request,
        CancellationToken cancellationToken = default);

    Task<StudentQrTokenDto> GetStudentQrTokenAsync(
        int studentId,
        CancellationToken cancellationToken = default);
}
