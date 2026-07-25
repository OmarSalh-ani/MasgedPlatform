using AdminAPI.DTOs.Teachers;

namespace AdminAPI.Services.Interfaces;

public interface ITeacherFormService
{
    Task<TeacherDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<TeacherCircleOptionDto>> GetCirclesAsync(
        bool forGirls,
        CancellationToken cancellationToken = default);
    Task<List<TeacherMosqueOptionDto>> GetMosquesAsync(CancellationToken cancellationToken = default);
    Task<TeacherDto> CreateAsync(SaveTeacherRequestDto request, CancellationToken cancellationToken = default);
    Task<TeacherDto> UpdateAsync(
        int id,
        SaveTeacherRequestDto request,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
