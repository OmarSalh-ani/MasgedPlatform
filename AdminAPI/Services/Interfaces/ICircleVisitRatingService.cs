using AdminAPI.DTOs.CircleVisitRating;
using AdminAPI.DTOs.Common;

namespace AdminAPI.Services.Interfaces;

public interface ICircleVisitRatingService
{
    Task<PagedResultDto<CircleVisitRatingListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<CircleVisitRatingDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<CircleVisitRatingTeacherOptionDto>> GetTeachersAsync(
        CancellationToken cancellationToken = default);

    Task<List<CircleVisitRatingCircleOptionDto>> GetCirclesAsync(
        int teacherId,
        CancellationToken cancellationToken = default);

    Task<CircleVisitRatingVisitNumberDto> GetVisitNumberAsync(
        int teacherId,
        DateTime visitDate,
        CancellationToken cancellationToken = default);

    Task<CircleVisitRatingDetailDto> CreateAsync(
        CreateCircleVisitRatingRequestDto request,
        CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string ContentType, string FileName)> ExportPdfAsync(
        int id,
        CancellationToken cancellationToken = default);
}
