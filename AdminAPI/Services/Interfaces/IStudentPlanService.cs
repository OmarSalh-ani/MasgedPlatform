using AdminAPI.DTOs.StudentPlan;

namespace AdminAPI.Services.Interfaces;

public interface IStudentPlanService
{
    Task<StudentPlanFormDataDto> GetFormDataAsync(CancellationToken cancellationToken = default);

    Task<List<StudentPlanAyahDto>> GetAyahsAsync(int surahId, CancellationToken cancellationToken = default);

    Task<StudentPlanResolveDto> ResolveAsync(
        int studentId,
        int? planId,
        string? editKey,
        CancellationToken cancellationToken = default);

    Task<StudentPlanDetailDto> GetPlanDetailAsync(
        int studentId,
        int planId,
        CancellationToken cancellationToken = default);

    Task<StudentPlanEditPrefillDto?> GetEditPrefillAsync(
        string editKey,
        CancellationToken cancellationToken = default);

    Task<CreateStudentPlanResponseDto> CreatePlanAsync(
        CreateStudentPlanRequestDto request,
        CancellationToken cancellationToken = default);

    Task SavePlanAsync(SaveStudentPlanRequestDto request, CancellationToken cancellationToken = default);

    Task UpdateSingleItemAsync(
        UpdateStudentPlanItemRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteItemAsync(string editKey, CancellationToken cancellationToken = default);
}
