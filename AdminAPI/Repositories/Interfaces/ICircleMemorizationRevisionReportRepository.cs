using AdminAPI.DTOs.CircleMemorizationRevisionReport;

namespace AdminAPI.Repositories.Interfaces;

public interface ICircleMemorizationRevisionReportRepository
{
    Task<List<CircleMemorizationTeacherOptionDto>> GetTeachersAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default);

    Task<(string? TeacherName, List<(int Id, string Name)> Circles)?> GetTeacherContextAsync(
        int teacherId,
        bool isGirlTeacher,
        CancellationToken cancellationToken = default);

    Task<List<CirclePlanSegmentDto>> GetMemorizingSegmentsAsync(
        IReadOnlyList<int> circleIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<List<CirclePlanSegmentDto>> GetReviseSegmentsAsync(
        IReadOnlyList<int> circleIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<List<CirclePlanSegmentDto>> GetArchiveMemorizingSegmentsAsync(
        IReadOnlyList<int> circleIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<List<CirclePlanSegmentDto>> GetArchiveReviseSegmentsAsync(
        IReadOnlyList<int> circleIds,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}
