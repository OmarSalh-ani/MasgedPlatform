using System.Drawing;
using System.Globalization;
using System.Security.Claims;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MemorizationRevisionReportController(AppDbContext db) : ControllerBase
{
    private const string DefaultStatus = "قيد الأنتظار";

    /// <summary>
    /// Circle-level حفظ/مراجعة report for a date range (PDF or Excel).
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportCircleReport(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string format = "pdf",
        CancellationToken cancellationToken = default)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على حلقتك. يرجى تسجيل الدخول مرة أخرى."));

        if (fromDate == default || toDate == default)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى تحديد من تاريخ والى تاريخ."));

        var from = fromDate.Date;
        var to = toDate.Date;
        if (to < from)
            return this.ToActionResult(GlobalResponse.BadRequest("تاريخ النهاية يجب أن يكون بعد أو يساوي تاريخ البداية."));

        if ((to - from).TotalDays > 366)
            return this.ToActionResult(GlobalResponse.BadRequest("الحد الأقصى لفترة التقرير هو 365 يوم."));

        var formatKey = (format ?? "pdf").Trim().ToLowerInvariant();
        if (formatKey is not ("pdf" or "excel" or "xlsx"))
            return this.ToActionResult(GlobalResponse.BadRequest("صيغة التقرير غير صالحة. استخدم pdf أو excel."));

        var circleName = await db.QuranCircles
            .AsNoTracking()
            .Where(c => c.Id == circleId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var teacherName = User.FindFirstValue("name") ?? "المعلم";

        // Filter on the same date the report groups by, otherwise a row assessed outside the
        // requested range would be pulled in by its planned date and shown under another day.
        var memorizings = await db.StudentPlanMemorizings
            .AsNoTracking()
            .Include(x => x.QuranSurah)
            .Include(x => x.RegisterForm)
            .Where(x => x.RegisterForm.QuranCircleId == circleId
                        && (x.MemorizeDate ?? x.PlanDate) >= from
                        && (x.MemorizeDate ?? x.PlanDate) <= to
                        && x.Status != null
                        && PlanRowStatus.CircleReportStatuses.Contains(x.Status))
            .ToListAsync(cancellationToken);

        var revises = await db.StudentPlanRevises
            .AsNoTracking()
            .Include(x => x.QuranSurah)
            .Include(x => x.RegisterForm)
            .Where(x => x.RegisterForm.QuranCircleId == circleId
                        && (x.ReviseDate ?? x.PlanDate) >= from
                        && (x.ReviseDate ?? x.PlanDate) <= to
                        && x.Status != null
                        && PlanRowStatus.CircleReportStatuses.Contains(x.Status))
            .ToListAsync(cancellationToken);

        var archiveCards = await db.StudentMemorizingCards
            .AsNoTracking()
            .Include(x => x.RegisterForm)
            .Where(x => x.CircleId == circleId
                        && x.CreatedAt.Date >= from
                        && x.CreatedAt.Date <= to
                        && (
                            (x.TheType == "حفظ" && x.IsSaveDone == "نعم")
                            || (x.TheType == "مراجعة" && x.IsDone == "نعم")))
            .ToListAsync(cancellationToken);

        var rows = CircleMemorizationRevisionReportBuilder.BuildRows(memorizings, revises, archiveCards);
        if (rows.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("لا توجد بيانات حفظ أو مراجعة في الفترة المحددة"));

        var meta = new CircleMemorizationRevisionReportMetaDto
        {
            CircleName = circleName,
            TeacherName = teacherName,
            PrintedAt = KuwaitTime.Now,
            FromDate = from,
            ToDate = to,
            Rows = rows,
        };

        byte[] bytes;
        string contentType;
        string extension;

        try
        {
            if (formatKey is "excel" or "xlsx")
            {
                bytes = CircleMemorizationRevisionReportExcelExporter.Build(meta);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                extension = "xlsx";
            }
            else
            {
                bytes = CircleMemorizationRevisionReportPdfExporter.Build(meta);
                contentType = "application/pdf";
                extension = "pdf";
            }
        }
        catch (Exception)
        {
            return this.ToActionResult(GlobalResponse.BadRequest("تعذر توليد التقرير. يرجى المحاولة مرة أخرى."));
        }

        if (bytes.Length == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("تعذر توليد التقرير. يرجى المحاولة مرة أخرى."));

        var fileName = ReportFileDownloadHelper.BuildCircleMemorizationReportFileName(extension);
        return ReportFileDownloadHelper.Create(bytes, contentType, fileName);
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على حلقتك. يرجى تسجيل الدخول مرة أخرى."));

        var currentIds = await db.RegisterForms
            .AsNoTracking()
            .Where(x => x.QuranCircleId == circleId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var formerIds = await db.StudentCircleEnrollments
            .AsNoTracking()
            .Where(e => e.CircleId == circleId && e.EndDate != null)
            .Select(e => e.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var allIds = currentIds.Union(formerIds).Distinct().ToList();

        var rawList = await db.RegisterForms
            .AsNoTracking()
            .Where(x => allIds.Contains(x.Id))
            .Select(x => new { x.Id, x.StudentName })
            .ToListAsync(cancellationToken);

        var list = rawList
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .OrderBy(x => x.StudentName)
            .ToList();

        var duplicateTrimmedNames = list
            .Select(x => (x.StudentName ?? "").Trim())
            .GroupBy(n => n, StringComparer.Ordinal)
            .Where(g => g.Count() > 1 && !string.IsNullOrEmpty(g.Key))
            .Select(g => g.Key)
            .ToHashSet(StringComparer.Ordinal);

        var students = list.Select(s => new MemorizationRevisionStudentPickDto
        {
            Id = s.Id,
            StudentName = s.StudentName,
            Label = FormatStudentPickLabel(s.StudentName, s.Id, duplicateTrimmedNames)
        }).ToList();

        return this.ToActionResult(GlobalResponse.Ok(students));
    }

    [HttpGet("{studentId:int}")]
    public async Task<IActionResult> GetReport(int studentId, CancellationToken cancellationToken)
    {
        if (studentId <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("معرّف الطالب غير صالح."));

        var access = await ValidateStudentAccessAsync(studentId, cancellationToken);
        if (!access.IsValid)
            return this.ToActionResult(GlobalResponse.BadRequest(access.Error));

        var rows = await BuildRowsAsync(studentId, cancellationToken);
        var studentName = await db.RegisterForms
            .AsNoTracking()
            .Where(r => r.Id == studentId)
            .Select(r => r.StudentName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        return this.ToActionResult(GlobalResponse.Ok(new MemorizationRevisionReportResponseDto
        {
            StudentId = studentId,
            StudentName = studentName,
            Rows = rows
        }));
    }

    [HttpGet("{studentId:int}/export")]
    public async Task<IActionResult> ExportToExcel(int studentId, CancellationToken cancellationToken)
    {
        if (studentId <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("معرّف الطالب غير صالح."));

        var access = await ValidateStudentAccessAsync(studentId, cancellationToken);
        if (!access.IsValid)
            return this.ToActionResult(GlobalResponse.BadRequest(access.Error));

        var rows = await BuildRowsAsync(studentId, cancellationToken);
        if (rows.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("لا توجد بيانات حفظ أو مراجعة لهذا الطالب."));

        var studentName = await db.RegisterForms
            .AsNoTracking()
            .Where(r => r.Id == studentId)
            .Select(r => r.StudentName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("تقرير الحفظ والمراجعة");
        ws.View.RightToLeft = true;

        ws.Cells[1, 1, 1, 6].Merge = true;
        ws.Cells[1, 1].Value = "تقرير الحفظ والمراجعة";
        ws.Cells[1, 1].Style.Font.Size = 16;
        ws.Cells[1, 1].Style.Font.Bold = true;
        ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        ws.Cells[2, 1, 2, 6].Merge = true;
        ws.Cells[2, 1].Value = "الطالب: " + studentName + " | " + KuwaitTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 4;
        ws.Cells[headerRow, 1].Value = "الحالة";
        ws.Cells[headerRow, 2].Value = "أسم السورة";
        ws.Cells[headerRow, 3].Value = "الطالب";
        ws.Cells[headerRow, 4].Value = "من الآية";
        ws.Cells[headerRow, 5].Value = "إلى الآية";
        ws.Cells[headerRow, 6].Value = "نوع الخطة";

        using (var hr = ws.Cells[headerRow, 1, headerRow, 6])
        {
            hr.Style.Font.Bold = true;
            hr.Style.Fill.PatternType = ExcelFillStyle.Solid;
            hr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(33, 118, 166));
            hr.Style.Font.Color.SetColor(Color.White);
            hr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        var r = headerRow + 1;
        foreach (var row in rows)
        {
            ws.Cells[r, 1].Value = row.Status;
            ws.Cells[r, 2].Value = row.SurahNameAr;
            ws.Cells[r, 3].Value = row.StudentName;
            ws.Cells[r, 4].Value = row.FromAyah;
            ws.Cells[r, 5].Value = row.ToAyah;
            ws.Cells[r, 6].Value = row.PlanType;
            r++;
        }

        ws.Cells.AutoFitColumns();

        var fileName = "تقرير_الحفظ_والمراجعة_" + studentId + "_" + KuwaitTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx";

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            fileData = Convert.ToBase64String(package.GetAsByteArray()),
            fileName
        }));
    }

    [HttpGet("{studentId:int}/export-completed-surahs")]
    public async Task<IActionResult> ExportCompletedSurahsExcel(int studentId, CancellationToken cancellationToken)
    {
        if (studentId <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("معرّف الطالب غير صالح."));

        var access = await ValidateStudentAccessAsync(studentId, cancellationToken);
        if (!access.IsValid)
            return this.ToActionResult(GlobalResponse.BadRequest(access.Error));

        var rows = await BuildCompletedSurahSummaryAsync(studentId, cancellationToken);
        if (rows.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("لا توجد سجلات بحالة «تم» لهذا الطالب."));

        var studentName = await db.RegisterForms
            .AsNoTracking()
            .Where(r => r.Id == studentId)
            .Select(r => r.StudentName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("السور التي تمت");
        ws.View.RightToLeft = true;

        ws.Cells[1, 1, 1, 6].Merge = true;
        ws.Cells[1, 1].Value = "تصدير السور التي تمت فقط (من سجل الخطة)";
        ws.Cells[1, 1].Style.Font.Size = 16;
        ws.Cells[1, 1].Style.Font.Bold = true;
        ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        ws.Cells[2, 1, 2, 6].Merge = true;
        ws.Cells[2, 1].Value = "الطالب: " + studentName + " | " + KuwaitTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 4;
        ws.Cells[headerRow, 1].Value = "اسم الطالب";
        ws.Cells[headerRow, 2].Value = "اسم السورة";
        ws.Cells[headerRow, 3].Value = "من الآية";
        ws.Cells[headerRow, 4].Value = "إلى الآية";
        ws.Cells[headerRow, 5].Value = "من التاريخ";
        ws.Cells[headerRow, 6].Value = "إلى التاريخ";

        using (var hr = ws.Cells[headerRow, 1, headerRow, 6])
        {
            hr.Style.Font.Bold = true;
            hr.Style.Fill.PatternType = ExcelFillStyle.Solid;
            hr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(33, 118, 166));
            hr.Style.Font.Color.SetColor(Color.White);
            hr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        var r = headerRow + 1;
        foreach (var row in rows)
        {
            ws.Cells[r, 1].Value = row.StudentName;
            ws.Cells[r, 2].Value = row.SurahNameAr;
            ws.Cells[r, 3].Value = row.FromAyah;
            ws.Cells[r, 4].Value = row.ToAyah;
            ws.Cells[r, 5].Value = row.FromDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            ws.Cells[r, 6].Value = row.ToDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            r++;
        }

        ws.Cells.AutoFitColumns();

        var fileName = "السور_التي_تمت_" + studentId + "_" + KuwaitTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx";

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            fileData = Convert.ToBase64String(package.GetAsByteArray()),
            fileName
        }));
    }

    private async Task<(bool IsValid, string Error)> ValidateStudentAccessAsync(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return (false, "لم يتم العثور على حلقتك. يرجى تسجيل الدخول مرة أخرى.");

        if (!await StudentCircleAccessHelper.CanReadStudentAsync(db, studentId, circleId, cancellationToken))
            return (false, "لا يمكنك عرض أو تصدير بيانات هذا الطالب.");

        return (true, string.Empty);
    }

    private async Task<List<PlanReportRowDto>> BuildRowsAsync(int studentId, CancellationToken cancellationToken)
    {
        var memList = await db.StudentPlanMemorizings
            .AsNoTracking()
            .Include(x => x.QuranSurah)
            .Include(x => x.RegisterForm)
            .Where(x => x.StudentId == studentId)
            .Select(x => new PlanReportRowDto
            {
                Status = x.Status ?? DefaultStatus,
                SurahNameAr = x.MemorizationLevel.StartsWith("__manual__:")
                    ? x.MemorizationLevel.Substring("__manual__:".Length)
                    : x.QuranSurah.NameAr,
                StudentName = x.RegisterForm.StudentName,
                FromAyah = x.FromAyahNumber,
                ToAyah = x.ToAyahNumber,
                PlanType = "خطة الحفظ"
            })
            .ToListAsync(cancellationToken);

        var revList = await db.StudentPlanRevises
            .AsNoTracking()
            .Include(x => x.QuranSurah)
            .Include(x => x.RegisterForm)
            .Where(x => x.StudentId == studentId)
            .Select(x => new PlanReportRowDto
            {
                Status = x.Status ?? DefaultStatus,
                SurahNameAr = x.MemorizationLevel.StartsWith("__manual__:")
                    ? x.MemorizationLevel.Substring("__manual__:".Length)
                    : x.QuranSurah.NameAr,
                StudentName = x.RegisterForm.StudentName,
                FromAyah = x.FromAyahNumber,
                ToAyah = x.ToAyahNumber,
                PlanType = "خطة المراجعة"
            })
            .ToListAsync(cancellationToken);

        return memList.Concat(revList).ToList();
    }

    private async Task<List<CompletedSurahSummaryRowDto>> BuildCompletedSurahSummaryAsync(
        int studentId,
        CancellationToken cancellationToken)
    {
        var logs = await db.StudentPlanItemLogs
            .AsNoTracking()
            .Where(x => x.StudentId == studentId
                && PlanRowStatus.CompletedStatuses.Contains(x.Status))
            .ToListAsync(cancellationToken);

        var resolved = new List<LogResolved>();
        foreach (var log in logs)
        {
            var resolvedRow = await TryResolvePlanRowAsync(studentId, log.RowKey, cancellationToken);
            if (resolvedRow is null)
                continue;

            resolved.Add(new LogResolved
            {
                Log = log,
                SurahId = resolvedRow.Value.SurahId,
                FromAyah = resolvedRow.Value.FromAyah,
                ToAyah = resolvedRow.Value.ToAyah
            });
        }

        if (resolved.Count == 0)
            return [];

        var studentName = await db.RegisterForms
            .AsNoTracking()
            .Where(r => r.Id == studentId)
            .Select(r => r.StudentName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var surahIds = resolved.Select(x => x.SurahId).Distinct().ToList();
        var surahNames = await db.QuranSurahs
            .AsNoTracking()
            .Where(q => surahIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id, q => q.NameAr ?? "", cancellationToken);

        var sortOrders = await db.QuranSurahs
            .AsNoTracking()
            .Where(q => surahIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id, q => q.SortOrder ?? int.MaxValue, cancellationToken);

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
                ToDate = latest.Log.LoggedAt
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

            var e = await db.StudentPlanMemorizings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == memId && x.StudentId == studentId, cancellationToken);

            if (e is null)
                return null;

            return (e.SurahId, e.FromAyahNumber, e.ToAyahNumber);
        }

        if (rowKey.StartsWith(revPrefix, StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(rowKey[revPrefix.Length..], out var revId))
                return null;

            var e = await db.StudentPlanRevises
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == revId && x.StudentId == studentId, cancellationToken);

            if (e is null)
                return null;

            return (e.SurahId, e.FromAyahNumber, e.ToAyahNumber);
        }

        return null;
    }

    private static string FormatStudentPickLabel(string studentName, int id, HashSet<string> duplicateTrimmedNames)
    {
        var trimmed = (studentName ?? "").Trim();
        if (string.IsNullOrEmpty(trimmed))
            return "طالب #" + id.ToString(CultureInfo.InvariantCulture);
        if (duplicateTrimmedNames.Contains(trimmed))
            return trimmed + " — #" + id.ToString(CultureInfo.InvariantCulture);
        return trimmed;
    }

    private bool TryGetCircleId(out int circleId)
    {
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }

    private sealed class LogResolved
    {
        public StudentPlanItemLog Log { get; set; } = null!;
        public int SurahId { get; set; }
        public int FromAyah { get; set; }
        public int ToAyah { get; set; }
    }
}
