using AdminAPI.DTOs.Teachers;
using ClosedXML.Excel;

namespace AdminAPI.Services;

public static class TeacherExcelExporter
{
    public static byte[] Build(IReadOnlyList<TeacherListItemDto> teachers)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("المعلمين");

        worksheet.Cell(1, 1).Value = "رقم المعلم";
        worksheet.Cell(1, 2).Value = "أسم المعلم";
        worksheet.Cell(1, 3).Value = "رقم الموبايل";
        worksheet.Cell(1, 4).Value = "البريد الألكتروني";
        worksheet.Cell(1, 5).Value = "كلمة المرور";
        worksheet.Cell(1, 6).Value = "أدمن عام";
        worksheet.Cell(1, 7).Value = "نوع المعلم";
        worksheet.Cell(1, 8).Value = "عدد الحلقات";

        var headerRange = worksheet.Range(1, 1, 1, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = XLFillPatternValues.Solid;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(33, 118, 166);
        headerRange.Style.Font.FontColor = XLColor.White;

        var row = 2;
        foreach (var teacher in teachers)
        {
            worksheet.Cell(row, 1).Value = teacher.Id;
            worksheet.Cell(row, 2).Value = teacher.Name;
            worksheet.Cell(row, 3).Value = teacher.Mobile;
            worksheet.Cell(row, 4).Value = teacher.Email;
            worksheet.Cell(row, 5).Value = teacher.Password;
            worksheet.Cell(row, 6).Value = teacher.UsersManage ? "نعم" : "لا";
            worksheet.Cell(row, 7).Value = teacher.CircleCount;
            row++;
        }

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
