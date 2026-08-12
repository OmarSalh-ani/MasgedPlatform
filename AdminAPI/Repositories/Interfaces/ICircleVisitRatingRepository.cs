using AdminAPI.DTOs.CircleVisitRating;
using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ICircleVisitRatingRepository
{
    Task<(List<CircleVisitRatingListItemDto> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool isAdmin,
        int currentTeacherId,
        CancellationToken cancellationToken = default);

    Task<CircleVisitRating?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);

    Task<string?> GetTeacherNameAsync(int teacherId, CancellationToken cancellationToken = default);

    Task<List<CircleVisitRatingTeacherOptionDto>> GetTeachersAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default);

    Task<List<CircleVisitRatingCircleOptionDto>> GetCirclesForTeacherAsync(
        int teacherId,
        bool isGirlTeacher,
        CancellationToken cancellationToken = default);

    Task<bool> CircleBelongsToTeacherAsync(
        int circleId,
        int teacherId,
        CancellationToken cancellationToken = default);

    Task<int> CountVisitsForTeacherInMonthAsync(
        int teacherId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task AddAsync(CircleVisitRating entity, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
