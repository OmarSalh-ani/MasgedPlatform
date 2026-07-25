using AdminAPI.DTOs.Students2;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class Students2Service(
    IStudents2Repository repository,
    IOptions<PublicSiteOptions> publicSiteOptions) : IStudents2Service
{
    private const string QuranCirclesRegistrationType = "حلقات تحفيظ القرآن";
    private const string MrkzRegistrationType = "دورة العلامة محمد بن صالح العثيمين";
    private const string UnspecifiedLabel = "غير محدد";

    public async Task<Students2ResponseDto> GetStudentsAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var rows = await repository.GetStudentsAsync(searchTerm.Trim(), cancellationToken);
        var baseUrl = publicSiteOptions.Value.BaseUrl.TrimEnd('/');

        var items = rows.Select(row => new Students2ListItemDto
        {
            Id = row.Id,
            Name = row.StudentName,
            FatherName = string.IsNullOrWhiteSpace(row.FatherName) ? UnspecifiedLabel : row.FatherName,
            Age = row.Age,
            Gender = row.StudentGender,
            FatherPhone = row.FatherPhone,
            CircleName = string.IsNullOrWhiteSpace(row.CircleName) ? UnspecifiedLabel : row.CircleName,
            RegistrationType =  QuranCirclesRegistrationType,
            RegistrationDate = row.CreatedAt ?? KuwaitTime.Now,
            ImageUrl = BuildPhotoUrl(row.PhotoPath, baseUrl) ?? string.Empty,
        }).ToList();

        return new Students2ResponseDto
        {
            Items = items,
            Stats = BuildStats(items),
        };
    }

    private static Students2StatsDto BuildStats(IReadOnlyList<Students2ListItemDto> items)
    {
        return new Students2StatsDto
        {
            TotalStudents = items.Count,
            MaleStudents = items.Count(x => x.Gender == "ذكر"),
            FemaleStudents = items.Count(x => x.Gender == "أنثى"),
        };
    }

    private static string? BuildPhotoUrl(string? photoPath, string publicSiteBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(photoPath))
            return null;

        var path = photoPath.Replace("~", string.Empty, StringComparison.Ordinal);
        return $"{publicSiteBaseUrl.TrimEnd('/')}{path}";
    }
}
