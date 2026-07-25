using System.Drawing;
using System.Globalization;
using System.Security.Claims;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
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
public class SpecialStudentsReportController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSpecialStudents(
        [FromQuery] bool isElite = false,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على معرف الحلقة. يرجى تسجيل الدخول مرة أخرى."));

        var students = await LoadStudentsAsync(circleId, isElite, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new SpecialStudentsReportResponseDto
        {
            IsElite = isElite,
            ReportTitle = isElite ? "تقرير طلاب النخبة" : "تقرير الطلاب المميزين",
            HasStudents = students.Count > 0,
            Students = students
        }));
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportSpecialStudentsToExcel(
        [FromQuery] bool isElite = false,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على معرف الحلقة. يرجى تسجيل الدخول مرة أخرى."));

        var students = await LoadStudentsAsync(circleId, isElite, cancellationToken);

        if (students.Count == 0)
        {
            var errorMessage = isElite ? "لا يوجد طلاب نخبة لتصديرهم" : "لا يوجد طلاب مميزين لتصديرهم";
            return this.ToActionResult(GlobalResponse.BadRequest(errorMessage));
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheetName = isElite ? "طلاب النخبة" : "الطلاب المميزين";
        var worksheet = package.Workbook.Worksheets.Add(worksheetName);

        worksheet.View.RightToLeft = true;

        worksheet.Cells[1, 1, 1, 6].Merge = true;
        var reportTitle = isElite ? "تقرير طلاب النخبة" : "تقرير الطلاب المميزين";
        worksheet.Cells[1, 1].Value = reportTitle;
        worksheet.Cells[1, 1].Style.Font.Size = 18;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Black);

        worksheet.Cells[2, 1, 2, 6].Merge = true;
        worksheet.Cells[2, 1].Value = $"تاريخ التقرير: {KuwaitTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
        worksheet.Cells[2, 1].Style.Font.Size = 12;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 4;
        worksheet.Cells[headerRow, 1].Value = "اسم الطالب";
        worksheet.Cells[headerRow, 2].Value = "الحلقة";
        worksheet.Cells[headerRow, 3].Value = "هاتف الوالد";
        worksheet.Cells[headerRow, 4].Value = "حالة الصورة";
        worksheet.Cells[headerRow, 5].Value = "رابط الصورة";
        worksheet.Cells[headerRow, 6].Value = "ملاحظات";

        var headerRange = worksheet.Cells[headerRow, 1, headerRow, 6];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.Size = 12;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        headerRange.Style.Font.Color.SetColor(Color.Black);
        headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thick;

        var dataRow = headerRow + 1;
        foreach (var student in students)
        {
            worksheet.Cells[dataRow, 1].Value = student.StudentName;
            worksheet.Cells[dataRow, 2].Value = student.CircleName;
            worksheet.Cells[dataRow, 3].Value = student.FatherPhone;

            if (!string.IsNullOrEmpty(student.ImageUrl))
            {
                worksheet.Cells[dataRow, 4].Value = "صورة متوفرة";
                worksheet.Cells[dataRow, 5].Value = student.ImageUrl;
            }
            else
            {
                worksheet.Cells[dataRow, 4].Value = "لا توجد صورة";
                worksheet.Cells[dataRow, 5].Value = "غير متوفر";
            }

            worksheet.Cells[dataRow, 6].Value = student.BadgeText;

            var rowRange = worksheet.Cells[dataRow, 1, dataRow, 6];
            rowRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            dataRow++;
        }

        var dataRange = worksheet.Cells[headerRow, 1, Math.Max(headerRow, dataRow - 1), 6];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Top.Color.SetColor(Color.Black);
        dataRange.Style.Border.Bottom.Color.SetColor(Color.Black);
        dataRange.Style.Border.Left.Color.SetColor(Color.Black);
        dataRange.Style.Border.Right.Color.SetColor(Color.Black);

        worksheet.Cells.AutoFitColumns();
        worksheet.Column(1).Width = Math.Max(worksheet.Column(1).Width, 25);
        worksheet.Column(2).Width = Math.Max(worksheet.Column(2).Width, 20);
        worksheet.Column(3).Width = Math.Max(worksheet.Column(3).Width, 15);
        worksheet.Column(4).Width = Math.Max(worksheet.Column(4).Width, 15);
        worksheet.Column(5).Width = Math.Max(worksheet.Column(5).Width, 40);
        worksheet.Column(6).Width = Math.Max(worksheet.Column(6).Width, 15);

        var fileBytes = package.GetAsByteArray();
        var fileNamePrefix = isElite ? "تقرير_طلاب_النخبة" : "تقرير_الطلاب_المميزين";
        var fileName = $"{fileNamePrefix}_{KuwaitTime.Now:yyyy-MM-dd}.xlsx";

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            fileData = Convert.ToBase64String(fileBytes),
            fileName,
            message = "تم إنشاء ملف Excel بنجاح",
            studentsCount = students.Count
        }));
    }

    private async Task<List<SpecialStudentReportDto>> LoadStudentsAsync(
        int circleId,
        bool isElite,
        CancellationToken cancellationToken)
    {
        var students = await db.RegisterForms
            .AsNoTracking()
            .Include(x => x.QuranCircle)
            .Include(x => x.ParentFollowup)
            .Where(x => (isElite ? x.IsElite : x.IsSpecial) && x.QuranCircleId == circleId)
            .OrderBy(x => x.StudentName)
            .ToListAsync(cancellationToken);

        return students.Select(x => new SpecialStudentReportDto
        {
            StudentName = x.StudentName,
            CircleName = x.QuranCircle?.Name ?? string.Empty,
            FatherPhone = x.FatherPhone,
                ImageUrl = x.ParentFollowup?.photoPath is not null and not ""
                    ? MediaUrlHelper.Resolve(x.ParentFollowup!.photoPath)
                    : string.Empty,
            BadgeText = isElite ? "طالب نخبة" : "طالب مميز",
            IsElite = isElite
        }).ToList();
    }

    private bool TryGetCircleId(out int circleId)
    {
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }
}
