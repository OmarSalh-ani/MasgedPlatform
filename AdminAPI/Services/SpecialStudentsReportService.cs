using System.Globalization;
using AdminAPI.DTOs.SpecialStudentsReport;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class SpecialStudentsReportService(
    ISpecialStudentsReportRepository repository,
    IOptions<PublicSiteOptions> publicSiteOptions) : ISpecialStudentsReportService
{
    public async Task<SpecialStudentsReportResponseDto> GetReportAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await repository.GetSpecialStudentsAsync(cancellationToken);
        var baseUrl = publicSiteOptions.Value.BaseUrl.TrimEnd('/');

        var items = rows.Select(row => new SpecialStudentsReportItemDto
        {
            StudentName = row.StudentName,
            CircleName = row.CircleName,
            FatherPhone = row.FatherPhone,
            ImageUrl = BuildPhotoUrl(row.PhotoPath, baseUrl) ?? string.Empty,
            CircleId = row.CircleId,
        }).ToList();

        return new SpecialStudentsReportResponseDto
        {
            Items = items,
            Stats = BuildStats(rows),
        };
    }

    public async Task<(byte[] Bytes, string FileName, int StudentsCount, int CirclesCount)?> ExportAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await repository.GetSpecialStudentsAsync(cancellationToken);
        if (rows.Count == 0)
            return null;

        var baseUrl = publicSiteOptions.Value.BaseUrl.TrimEnd('/');
        var exportRows = rows.Select(row => new SpecialStudentsReportExportRowDto
        {
            StudentName = row.StudentName,
            CircleName = row.CircleName,
            FatherPhone = row.FatherPhone,
            FatherPhone2 = row.FatherPhone2,
            StudentPhone = row.StudentPhone,
            StudentGender = row.StudentGender,
            Age = row.Age,
            HasImage = !string.IsNullOrEmpty(BuildPhotoUrl(row.PhotoPath, baseUrl)),
        }).ToList();

        var uniqueCircles = rows.Select(x => x.CircleId).Distinct().Count();
        var bytes = SpecialStudentsReportExcelExporter.Build(exportRows, rows.Count, uniqueCircles);
        var fileName =
            "تقرير_الطلاب_المميزين_جميع_الحلقات_" +
            KuwaitTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".xlsx";

        return (bytes, fileName, rows.Count, uniqueCircles);
    }

    private static SpecialStudentsReportStatsDto BuildStats(IReadOnlyList<SpecialStudentsReportRowDto> rows)
    {
        var totalStudents = rows.Count;
        var uniqueCircles = rows.Select(x => x.CircleId).Distinct().Count();
        var averagePerCircle = uniqueCircles > 0
            ? Math.Round((double)totalStudents / uniqueCircles, 1)
            : 0;

        return new SpecialStudentsReportStatsDto
        {
            TotalStudents = totalStudents,
            TotalCircles = uniqueCircles,
            AveragePerCircle = averagePerCircle,
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
