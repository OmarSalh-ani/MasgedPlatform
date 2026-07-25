using AdminAPI.DTOs.ParentsFollowup;

namespace AdminAPI.Services.Interfaces;

public interface IParentsFollowupService
{
    Task<ParentsFollowupDto?> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task SubmitAsync(int studentId, SaveParentsFollowupRequestDto request, CancellationToken cancellationToken = default);
}
