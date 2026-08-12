using System.Globalization;
using AdminAPI.DTOs.CircleMemorizationRevisionReport;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class CircleMemorizationRevisionReportService(
    ICircleMemorizationRevisionReportRepository repository,
    ICurrentUserContext currentUser) : ICircleMemorizationRevisionReportService
{
    public Task<List<CircleMemorizationTeacherOptionDto>> GetTeachersAsync(
        CancellationToken cancellationToken = default) =>
        repository.GetTeachersAsync(currentUser.IsGirlTeacher, cancellationToken);

    public async Task<(byte[] Bytes, string FileName, string ContentType)?> ExportAsync(
        int teacherId,
        DateTime fromDate,
        DateTime toDate,
        string format,
        CancellationToken cancellationToken = default)
    {
        var from = fromDate.Date;
        var to = toDate.Date;
        if (teacherId <= 0 || from == default || to == default || to < from)
            return null;

        if ((to - from).TotalDays > 366)
            return null;

        var context = await repository.GetTeacherContextAsync(
            teacherId, currentUser.IsGirlTeacher, cancellationToken);
        if (context is null || context.Value.Circles.Count == 0)
            return null;

        var circleIds = context.Value.Circles.Select(c => c.Id).ToList();
        var circleName = string.Join("، ", context.Value.Circles.Select(c => c.Name).Where(n => !string.IsNullOrWhiteSpace(n)));

        var mem = await repository.GetMemorizingSegmentsAsync(circleIds, from, to, cancellationToken);
        var rev = await repository.GetReviseSegmentsAsync(circleIds, from, to, cancellationToken);
        var archiveMem = await repository.GetArchiveMemorizingSegmentsAsync(circleIds, from, to, cancellationToken);
        var archiveRev = await repository.GetArchiveReviseSegmentsAsync(circleIds, from, to, cancellationToken);
        var rows = CircleMemorizationRevisionReportBuilder.BuildRows(
            [.. mem, .. archiveMem],
            [.. rev, .. archiveRev]);
        if (rows.Count == 0)
            return null;

        var meta = new CircleMemorizationRevisionReportMetaDto
        {
            CircleName = circleName,
            TeacherName = context.Value.TeacherName ?? "",
            PrintedAt = KuwaitTime.Now,
            FromDate = from,
            ToDate = to,
            Rows = rows,
        };

        var formatKey = (format ?? "pdf").Trim().ToLowerInvariant();
        var stamp = KuwaitTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

        if (formatKey is "excel" or "xlsx")
        {
            return (
                CircleMemorizationRevisionReportExcelExporter.Build(meta),
                "تقرير_الحفظ_والمراجعة_" + stamp + ".xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        return (
            CircleMemorizationRevisionReportPdfExporter.Build(meta),
            "تقرير_الحفظ_والمراجعة_" + stamp + ".pdf",
            "application/pdf");
    }
}
