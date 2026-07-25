using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IParentsFollowupRepository
{
    Task<RegisterForm?> GetStudentWithFollowupAsync(int studentId, CancellationToken cancellationToken = default);
}
