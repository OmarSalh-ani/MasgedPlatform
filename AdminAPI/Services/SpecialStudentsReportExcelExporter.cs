using System.Globalization;
using AdminAPI.DTOs.SpecialStudentsReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class SpecialStudentsReportExcelExporter
{
    public static byte[] Build(
        IReadOnlyList<SpecialStudentsReportExportRowDto> rows,
        int totalStudents,
        int uniqueCircles)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("الطلاب المميزين - جميع الحلقات");
        worksheet.View.RightToLeft = true;

        worksheet.Cells[1, 1, 1, 8].Merge = true;
        worksheet.Cells[1, 1].Value = "تقرير الطلاب المميزين - جميع الحلقات";
        worksheet.Cells[1, 1].Style.Font.Size = 18;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Black);

        worksheet.Cells[2, 1, 2, 8].Merge = true;
        worksheet.Cells[2, 1].Value =
            $"تاريخ التقرير: {FormatDateGeorgian(KuwaitTime.Now)} | إجمالي الطلاب: {totalStudents} | عدد الحلقات: {uniqueCircles}";
        worksheet.Cells[2, 1].Style.Font.Size = 12;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 4;
        worksheet.Cells[headerRow, 1].Value = "اسم الطالب";
        worksheet.Cells[headerRow, 2].Value = "الحلقة";
        worksheet.Cells[headerRow, 3].Value = "هاتف الوالد";
        worksheet.Cells[headerRow, 4].Value = "هاتف الوالد الثاني";
        worksheet.Cells[headerRow, 5].Value = "هاتف الطالب";
        worksheet.Cells[headerRow, 6].Value = "الجنس";
        worksheet.Cells[headerRow, 7].Value = "العمر";
        worksheet.Cells[headerRow, 8].Value = "حالة الصورة";

        StyleHeaderRow(worksheet, headerRow, 8);

        var dataRow = headerRow + 1;
        foreach (var student in rows)
        {
            worksheet.Cells[dataRow, 1].Value = student.StudentName;
            worksheet.Cells[dataRow, 2].Value = student.CircleName;
            worksheet.Cells[dataRow, 3].Value = student.FatherPhone;
            worksheet.Cells[dataRow, 4].Value = student.FatherPhone2 ?? "غير متوفر";
            worksheet.Cells[dataRow, 5].Value = student.StudentPhone ?? "غير متوفر";
            worksheet.Cells[dataRow, 6].Value = student.StudentGender;
            worksheet.Cells[dataRow, 7].Value = student.Age;
            worksheet.Cells[dataRow, 8].Value = student.HasImage ? "صورة متوفرة" : "لا توجد صورة";

            using var rowRange = worksheet.Cells[dataRow, 1, dataRow, 8];
            rowRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            dataRow++;
        }

        ApplyDataBorders(worksheet, headerRow, dataRow - 1, 8);
        worksheet.Cells.AutoFitColumns();
        SetMinimumColumnWidths(worksheet);

        return package.GetAsByteArray();
    }

    private static void StyleHeaderRow(ExcelWorksheet worksheet, int headerRow, int columnCount)
    {
        using var headerRange = worksheet.Cells[headerRow, 1, headerRow, columnCount];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.Size = 12;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        headerRange.Style.Font.Color.SetColor(Color.Black);
        headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thick;
    }

    private static void ApplyDataBorders(ExcelWorksheet worksheet, int headerRow, int lastRow, int columnCount)
    {
        using var dataRange = worksheet.Cells[headerRow, 1, lastRow, columnCount];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Top.Color.SetColor(Color.Black);
        dataRange.Style.Border.Bottom.Color.SetColor(Color.Black);
        dataRange.Style.Border.Left.Color.SetColor(Color.Black);
        dataRange.Style.Border.Right.Color.SetColor(Color.Black);
    }

    private static void SetMinimumColumnWidths(ExcelWorksheet worksheet)
    {
        worksheet.Column(1).Width = Math.Max(worksheet.Column(1).Width, 25);
        worksheet.Column(2).Width = Math.Max(worksheet.Column(2).Width, 20);
        worksheet.Column(3).Width = Math.Max(worksheet.Column(3).Width, 15);
        worksheet.Column(4).Width = Math.Max(worksheet.Column(4).Width, 15);
        worksheet.Column(5).Width = Math.Max(worksheet.Column(5).Width, 15);
        worksheet.Column(6).Width = Math.Max(worksheet.Column(6).Width, 10);
        worksheet.Column(7).Width = Math.Max(worksheet.Column(7).Width, 10);
        worksheet.Column(8).Width = Math.Max(worksheet.Column(8).Width, 15);
    }

    private static string FormatDateGeorgian(DateTime date) =>
        date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
}
