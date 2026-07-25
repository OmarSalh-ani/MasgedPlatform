using System.Globalization;
using AdminAPI.DTOs.MemorizationRevisionReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class MemorizationRevisionReportExcelExporter
{
    public static byte[] BuildFullReport(string studentName, IReadOnlyList<MemorizationRevisionPlanRowDto> rows)
    {
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
        ws.Cells[2, 1].Value = "الطالب: " + studentName + " | " + FormatDateGeorgian(KuwaitTime.Now);
        ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 4;
        ws.Cells[headerRow, 1].Value = "الحالة";
        ws.Cells[headerRow, 2].Value = "أسم السورة";
        ws.Cells[headerRow, 3].Value = "الطالب";
        ws.Cells[headerRow, 4].Value = "من الآية";
        ws.Cells[headerRow, 5].Value = "إلى الآية";
        ws.Cells[headerRow, 6].Value = "نوع الخطة";

        StyleHeaderRow(ws, headerRow, 6);

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
        return package.GetAsByteArray();
    }

    public static byte[] BuildCompletedSurahs(string studentName, IReadOnlyList<CompletedSurahSummaryRowDto> rows)
    {
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
        ws.Cells[2, 1].Value = "الطالب: " + studentName + " | " + FormatDateGeorgian(KuwaitTime.Now);
        ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 4;
        ws.Cells[headerRow, 1].Value = "اسم الطالب";
        ws.Cells[headerRow, 2].Value = "اسم السورة";
        ws.Cells[headerRow, 3].Value = "من الآية";
        ws.Cells[headerRow, 4].Value = "إلى الآية";
        ws.Cells[headerRow, 5].Value = "من التاريخ";
        ws.Cells[headerRow, 6].Value = "إلى التاريخ";

        StyleHeaderRow(ws, headerRow, 6);

        var r = headerRow + 1;
        foreach (var row in rows)
        {
            ws.Cells[r, 1].Value = row.StudentName;
            ws.Cells[r, 2].Value = row.SurahNameAr;
            ws.Cells[r, 3].Value = row.FromAyah;
            ws.Cells[r, 4].Value = row.ToAyah;
            ws.Cells[r, 5].Value = FormatDateGeorgian(row.FromDate);
            ws.Cells[r, 6].Value = FormatDateGeorgian(row.ToDate);
            r++;
        }

        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    private static void StyleHeaderRow(ExcelWorksheet ws, int headerRow, int columnCount)
    {
        using var hr = ws.Cells[headerRow, 1, headerRow, columnCount];
        hr.Style.Font.Bold = true;
        hr.Style.Fill.PatternType = ExcelFillStyle.Solid;
        hr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(33, 118, 166));
        hr.Style.Font.Color.SetColor(Color.White);
        hr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    }

    private static string FormatDateGeorgian(DateTime date) =>
        date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
}
