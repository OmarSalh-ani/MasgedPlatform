using AdminAPI.DTOs.Students2;

namespace AdminAPI.Repositories.Interfaces;

public interface IStudents2Repository
{
    Task<List<Students2RowDto>> GetStudentsAsync(
        string searchTerm,
        CancellationToken cancellationToken = default);
}
