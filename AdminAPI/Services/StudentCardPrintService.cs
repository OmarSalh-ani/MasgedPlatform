using AdminAPI.Data;
using AdminAPI.DTOs.Students;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class StudentCardPrintService(
    AdminDbContext db,
    ICurrentUserContext currentUser,
    IOptions<PublicSiteOptions> publicSiteOptions) : IStudentCardPrintService
{
    public async Task<StudentCardPrintDto> GetCardPrintAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin)
            throw new UnauthorizedAccessException("غير مصرح");

        var student = await db.RegisterForms
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.StudentName,
                x.FatherPhone,
                CircleName = x.QuranCircle != null ? x.QuranCircle.Name : string.Empty,
                PhotoPath = x.ParentFollowup != null ? x.ParentFollowup.PhotoPath : null,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("الطالب غير موجود");

        var circleOptions = await db.QuranCircles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        return new StudentCardPrintDto
        {
            Id = student.Id,
            StudentName = student.StudentName,
            CircleName = student.CircleName,
            FatherMobile = student.FatherPhone,
            ImageUrl = BuildPhotoUrl(student.PhotoPath, publicSiteOptions.Value.BaseUrl),
            CircleOptions = circleOptions,
        };
    }

    private static string? BuildPhotoUrl(string? photoPath, string publicSiteBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(photoPath))
            return null;

        var path = photoPath.Replace("~", string.Empty);
        return $"{publicSiteBaseUrl.TrimEnd('/')}{path}";
    }
}
