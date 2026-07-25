using AdminAPI.DTOs.StudentPlan;
using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IStudentPlanRepository
{
    Task<List<QuranSurah>> GetSurahsAsync(CancellationToken cancellationToken = default);

    Task<List<StudentPlanCircleOptionDto>> GetCirclesAsync(
        bool isGirlTeacher,
        bool isAdmin,
        int teacherId,
        CancellationToken cancellationToken = default);

    Task<List<StudentPlanStudentOptionDto>> GetStudentsAsync(
        bool isGirlTeacher,
        bool isAdmin,
        int teacherId,
        CancellationToken cancellationToken = default);

    Task<List<int>> GetAyahNumbersAsync(int surahId, CancellationToken cancellationToken = default);

    Task<RegisterForm?> GetStudentAsync(int studentId, CancellationToken cancellationToken = default);

    Task<List<StudentPlan>> GetPlansForStudentAsync(int studentId, CancellationToken cancellationToken = default);

    Task<StudentPlan?> GetPlanAsync(int planId, CancellationToken cancellationToken = default);

    Task<List<StudentPlanMemorizing>> GetMemorizingsAsync(
        int studentId,
        int planId,
        CancellationToken cancellationToken = default);

    Task<List<StudentPlanRevise>> GetRevisesAsync(
        int studentId,
        int planId,
        CancellationToken cancellationToken = default);

    Task<int?> ResolvePlanIdFromEditKeyAsync(
        int studentId,
        string editKey,
        CancellationToken cancellationToken = default);

    Task<StudentPlanMemorizing?> GetMemorizingByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<StudentPlanRevise?> GetReviseByIdAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
