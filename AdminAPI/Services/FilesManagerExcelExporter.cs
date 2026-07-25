using AdminAPI.DTOs.FilesManager;
using ClosedXML.Excel;

namespace AdminAPI.Services;

public static class FilesManagerExcelExporter
{
    public static byte[] Export(IReadOnlyList<FilesManagerListItemDto> files)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Files");

        worksheet.Cell(1, 1).Value = "رقم الملف";
        worksheet.Cell(1, 2).Value = "اسم الملف";
        worksheet.Cell(1, 3).Value = "رابط الملف";

        var headerRange = worksheet.Range(1, 1, 1, 3);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = XLFillPatternValues.Solid;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

        var row = 2;
        foreach (var file in files)
        {
            worksheet.Cell(row, 1).Value = file.Id;
            worksheet.Cell(row, 2).Value = file.Name;
            worksheet.Cell(row, 3).Value = file.FileUrl;
            row++;
        }

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
