using AdminAPI.DTOs.EventPageResponses;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class EventPageResponseExcelExporter
{
    public static byte[] Build(
        IReadOnlyList<EventPageResponseListItemDto> items,
        IReadOnlyList<string> fieldLabels)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Responses");

        worksheet.Cells[1, 1].Value = "تاريخ التسجيل";
        worksheet.Cells[1, 2].Value = "اسم النشاط";

        for (var i = 0; i < fieldLabels.Count; i++)
            worksheet.Cells[1, i + 3].Value = fieldLabels[i];

        var lastCol = Math.Max(2, fieldLabels.Count + 2);
        using (var range = worksheet.Cells[1, 1, 1, lastCol])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        }

        var row = 2;
        foreach (var item in items)
        {
            worksheet.Cells[row, 1].Value = item.SubmittedAt.ToString("yyyy-MM-dd HH:mm");
            worksheet.Cells[row, 2].Value = item.ActivityName;

            for (var i = 0; i < fieldLabels.Count; i++)
            {
                var label = fieldLabels[i];
                worksheet.Cells[row, i + 3].Value = item.Values
                    .FirstOrDefault(v => v.FieldLabel == label)?.Value ?? string.Empty;
            }

            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}
