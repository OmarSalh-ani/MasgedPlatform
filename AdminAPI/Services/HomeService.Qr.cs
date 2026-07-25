using AdminAPI.DTOs.Home;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public partial class HomeService
{
    public async Task<StudentQrTokenDto> GetStudentQrTokenAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var teacherCircleIds = await GetTeacherCircleIdsAsync(cancellationToken);
        var gender = currentUser.IsGirlTeacher ? "أنثى" : "ذكر";

        var query = db.RegisterForms.AsNoTracking()
            .Where(x => x.Id == studentId && x.StudentGender == gender);

        if (!currentUser.IsAdmin)
        {
            query = query.Where(x =>
                x.QuranCircleId != null && teacherCircleIds.Contains(x.QuranCircleId.Value));
        }

        var exists = await query.AnyAsync(cancellationToken);
        if (!exists)
            throw new KeyNotFoundException("الطالب غير موجود");

        return new StudentQrTokenDto
        {
            Token = qrTokenService.EncryptStudentId(studentId),
        };
    }
}
