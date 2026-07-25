using System.Globalization;
using AdminAPI.DTOs.MemorizationRevisionReport;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class MemorizationRevisionReportService(IMemorizationRevisionReportRepository repository)
    : IMemorizationRevisionReportService
{
    private static readonly string[] LogStatusCompleted =
        ["تم", "تم الحفظ", "قيد الانتظار في التثبيت"];

    public Task<bool> StudentExistsAsync(int studentId, CancellationToken cancellationToken = default) =>
        repository.StudentExistsAsync(studentId, cancellationToken);

    public async Task<List<MemorizationRevisionStudentPickDto>> GetStudentsAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await repository.GetStudentPickListAsync(cancellationToken);

        var duplicateTrimmedNames = list
            .Select(x => (x.StudentName ?? string.Empty).Trim())
            .GroupBy(n => n, StringComparer.Ordinal)
            .Where(g => g.Count() > 1 && !string.IsNullOrEmpty(g.Key))
            .Select(g => g.Key)
            .ToHashSet(StringComparer.Ordinal);

        return list.Select(s => new MemorizationRevisionStudentPickDto
        {
            Id = s.Id,
            StudentName = s.StudentName ?? string.Empty,
            Label = FormatStudentPickLabel(s.StudentName, s.Id, duplicateTrimmedNames),
        }).ToList();
    }

    public async Task<MemorizationRevisionReportResponseDto?> GetReportAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        if (studentId <= 0 || !await repository.StudentExistsAsync(studentId, cancellationToken))
            return null;

        var rows = await repository.GetPlanRowsAsync(studentId, cancellationToken);
        var studentName = await repository.GetStudentNameAsync(studentId, cancellationToken) ?? string.Empty;

        return new MemorizationRevisionReportResponseDto
        {
            StudentId = studentId,
            StudentName = studentName,
            Rows = rows,
        };
    }

    public async Task<(byte[] Bytes, string FileName)?> ExportFullReportAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        if (studentId <= 0 || !await repository.StudentExistsAsync(studentId, cancellationToken))
            return null;

        var rows = await repository.GetPlanRowsAsync(studentId, cancellationToken);
        if (rows.Count == 0)
            return null;

        var studentName = await repository.GetStudentNameAsync(studentId, cancellationToken) ?? string.Empty;
        var bytes = MemorizationRevisionReportExcelExporter.BuildFullReport(studentName, rows);
        var fileName =
            "تقرير_الحفظ_والمراجعة_" + studentId + "_" +
            KuwaitTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx";

        return (bytes, fileName);
    }

    public async Task<(byte[] Bytes, string FileName)?> ExportCompletedSurahsAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        if (studentId <= 0 || !await repository.StudentExistsAsync(studentId, cancellationToken))
            return null;

        var rows = await BuildCompletedSurahSummaryAsync(studentId, cancellationToken);
        if (rows.Count == 0)
            return null;

        var studentName = await repository.GetStudentNameAsync(studentId, cancellationToken) ?? string.Empty;
        var bytes = MemorizationRevisionReportExcelExporter.BuildCompletedSurahs(studentName, rows);
        var fileName =
            "السور_التي_تمت_" + studentId + "_" +
            KuwaitTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx";

        return (bytes, fileName);
    }

    private async Task<List<CompletedSurahSummaryRowDto>> BuildCompletedSurahSummaryAsync(
        int studentId,
        CancellationToken cancellationToken)
    {
        var logs = await repository.GetCompletedLogsAsync(studentId, LogStatusCompleted, cancellationToken);
        var resolved = new List<LogResolved>();

        foreach (var log in logs)
        {
            var row = await TryResolvePlanRowAsync(studentId, log.RowKey, cancellationToken);
            if (row is null)
                continue;

            resolved.Add(new LogResolved
            {
                Log = log,
                SurahId = row.Value.SurahId,
                FromAyah = row.Value.FromAyah,
                ToAyah = row.Value.ToAyah,
            });
        }

        if (resolved.Count == 0)
            return [];

        var studentName = await repository.GetStudentNameAsync(studentId, cancellationToken) ?? string.Empty;
        var surahIds = resolved.Select(x => x.SurahId).Distinct().ToList();
        var surahNames = await repository.GetSurahNamesAsync(surahIds, cancellationToken);
        var sortOrders = await repository.GetSurahSortOrdersAsync(surahIds, cancellationToken);

        var result = new List<CompletedSurahSummaryRowDto>();
        foreach (var g in resolved.GroupBy(x => x.SurahId))
        {
            var oldest = g.OrderBy(x => x.Log.LoggedAt).First();
            var latest = g.OrderByDescending(x => x.Log.LoggedAt).First();
            surahNames.TryGetValue(g.Key, out var surahLabel);

            result.Add(new CompletedSurahSummaryRowDto
            {
                StudentName = studentName,
                SurahId = g.Key,
                SurahNameAr = surahLabel ?? "—",
                FromAyah = oldest.FromAyah,
                ToAyah = latest.ToAyah,
                FromDate = oldest.Log.LoggedAt,
                ToDate = latest.Log.LoggedAt,
            });
        }

        return result
            .OrderBy(x => sortOrders.TryGetValue(x.SurahId, out var so) ? so : int.MaxValue)
            .ThenBy(x => x.FromDate)
            .ToList();
    }

    private async Task<(int SurahId, int FromAyah, int ToAyah)?> TryResolvePlanRowAsync(
        int studentId,
        string rowKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(rowKey))
            return null;

        const string memPrefix = "memorizing_";
        const string revPrefix = "revise_";

        if (rowKey.StartsWith(memPrefix, StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(rowKey[memPrefix.Length..], out var memId))
                return null;

            var e = await repository.GetMemorizingByIdAsync(memId, studentId, cancellationToken);
            if (e is null)
                return null;

            return (e.SurahId, e.FromAyahNumber, e.ToAyahNumber);
        }

        if (rowKey.StartsWith(revPrefix, StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(rowKey[revPrefix.Length..], out var revId))
                return null;

            var e = await repository.GetReviseByIdAsync(revId, studentId, cancellationToken);
            if (e is null)
                return null;

            return (e.SurahId, e.FromAyahNumber, e.ToAyahNumber);
        }

        return null;
    }

    private static string FormatStudentPickLabel(
        string? studentName,
        int id,
        HashSet<string> duplicateTrimmedNames)
    {
        var trimmed = (studentName ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(trimmed))
            return "طالب #" + id.ToString(CultureInfo.InvariantCulture);
        if (duplicateTrimmedNames.Contains(trimmed))
            return trimmed + " — #" + id.ToString(CultureInfo.InvariantCulture);
        return trimmed;
    }

    private sealed class LogResolved
    {
        public StudentPlanItemLog Log { get; set; } = null!;
        public int SurahId { get; set; }
        public int FromAyah { get; set; }
        public int ToAyah { get; set; }
    }
}
