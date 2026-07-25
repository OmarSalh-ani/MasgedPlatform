using System.Globalization;
using AdminAPI.DTOs.Tests;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class TestsReportExcelExporter
{
    private const string ProgramType = "حلقات تحفيظ القرآن الكريم";
    private static readonly CultureInfo DisplayCulture = CultureInfo.InvariantCulture;

    public static byte[] Build(
        IReadOnlyList<TestsReportSourceRow> rows,
        DateTime fromDate,
        DateTime toDateInclusive)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("تقرير الاختبارات");
        worksheet.View.RightToLeft = true;

        worksheet.Cells[1, 1, 1, 12].Merge = true;
        worksheet.Cells[1, 1].Value = "تقرير الاختبارات";
        worksheet.Cells[1, 1].Style.Font.Size = 18;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        worksheet.Cells[2, 1, 2, 12].Merge = true;
        worksheet.Cells[2, 1].Value =
            $"من {fromDate:dd/MM/yyyy} إلى {toDateInclusive:dd/MM/yyyy}";
        worksheet.Cells[2, 1].Style.Font.Size = 12;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 5;
        string[] headers =
        [
            "رقم الطالب", "اسم الطالب", "هاتف الوالد", "اسم المعلم", "اسم الحلقة",
            "نوع البرنامج", "من", "إلى", "تاريخ الاختبار", "النتيجة النهائية", "ملاحظات", "نوع الاختبار",
        ];

        for (var col = 0; col < headers.Length; col++)
            worksheet.Cells[headerRow, col + 1].Value = headers[col];

        StyleHeader(worksheet, headerRow, headers.Length);

        var dataRow = headerRow + 1;
        foreach (var test in rows)
        {
            worksheet.Cells[dataRow, 1].Value = test.StudentId;
            worksheet.Cells[dataRow, 2].Value = test.StudentName;
            worksheet.Cells[dataRow, 3].Value = test.ParentPhone;
            worksheet.Cells[dataRow, 4].Value = test.TeacherName;
            worksheet.Cells[dataRow, 5].Value = test.CircleName;
            worksheet.Cells[dataRow, 6].Value = ProgramType;
            worksheet.Cells[dataRow, 7].Value = test.TestFrom;
            worksheet.Cells[dataRow, 8].Value = test.TestTo;
            worksheet.Cells[dataRow, 9].Value = test.TestDate.ToString("dd/MM/yyyy", DisplayCulture);
            worksheet.Cells[dataRow, 10].Value = test.FinalResults;
            worksheet.Cells[dataRow, 11].Value = test.Notes;
            worksheet.Cells[dataRow, 12].Value = test.TestName;

            using var rowRange = worksheet.Cells[dataRow, 1, dataRow, 12];
            rowRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            dataRow++;
        }

        if (dataRow > headerRow + 1)
        {
            using var dataRange = worksheet.Cells[headerRow, 1, dataRow - 1, 12];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        worksheet.Cells.AutoFitColumns();
        for (var i = 1; i <= 12; i++)
            worksheet.Column(i).Width = Math.Max(worksheet.Column(i).Width, 15);

        return package.GetAsByteArray();
    }

    private static void StyleHeader(ExcelWorksheet worksheet, int headerRow, int columnCount)
    {
        using var headerRange = worksheet.Cells[headerRow, 1, headerRow, columnCount];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.Size = 12;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thick;
    }
}
