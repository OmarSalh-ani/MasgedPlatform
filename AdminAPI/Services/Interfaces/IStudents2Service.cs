using AdminAPI.DTOs.Students2;

namespace AdminAPI.Services.Interfaces;

public interface IStudents2Service
{
    Task<Students2ResponseDto> GetStudentsAsync(
        string searchTerm,
        CancellationToken cancellationToken = default);
}
