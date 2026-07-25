using AdminAPI.DTOs.Students;

namespace AdminAPI.Services.Interfaces;

public interface IStudentCardPrintService
{
    Task<StudentCardPrintDto> GetCardPrintAsync(int id, CancellationToken cancellationToken = default);
}
