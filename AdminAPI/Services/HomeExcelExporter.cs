using ClosedXML.Excel;
using AdminAPI.DTOs.Home;

namespace AdminAPI.Services;

public static class HomeExcelExporter
{
    public static byte[] Build(IReadOnlyList<HomeExportRow> students)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Students");

        worksheet.Cell(1, 1).Value = "رقم الطالب";
        worksheet.Cell(1, 2).Value = "اسم الطالب";
        worksheet.Cell(1, 3).Value = "اسم الأب";
        worksheet.Cell(1, 4).Value = "تاريخ الميلاد";
        worksheet.Cell(1, 5).Value = "العمر";
        worksheet.Cell(1, 6).Value = "الجنس";
        worksheet.Cell(1, 7).Value = "هاتف ولي الأمر";
        worksheet.Cell(1, 8).Value = "هاتف ولي الأمر 2";
        worksheet.Cell(1, 9).Value = "هاتف الطالب";
        worksheet.Cell(1, 10).Value = "تاريخ التسجيل";
        worksheet.Cell(1, 11).Value = "الحلقة";
        worksheet.Cell(1, 12).Value = "طالب مميز";
        worksheet.Cell(1, 13).Value = "نوع النشاط";
        worksheet.Cell(1, 14).Value = "المؤهل العلمي";
        worksheet.Cell(1, 15).Value = "كلمة المرور";
        worksheet.Cell(1, 16).Value = "مرات الغياب";
        worksheet.Cell(1, 17).Value = "الاستمارة مكتملة";
        worksheet.Cell(1, 18).Value = "طالب نخبة";

        var row = 2;
        foreach (var student in students)
        {
            worksheet.Cell(row, 1).Value = student.Id;
            worksheet.Cell(row, 2).Value = student.StudentName;
            worksheet.Cell(row, 3).Value = student.FatherName;
            worksheet.Cell(row, 4).Value = GeorgianDateFormatter.FormatDate(student.Birthdate);
            worksheet.Cell(row, 5).Value = student.Age;
            worksheet.Cell(row, 6).Value = student.StudentGender;
            worksheet.Cell(row, 7).Value = student.FatherPhone;
            worksheet.Cell(row, 8).Value = student.FatherPhone2;
            worksheet.Cell(row, 9).Value = student.StudentPhone;
            worksheet.Cell(row, 10).Value = GeorgianDateFormatter.FormatDate(student.CreatedAt);
            worksheet.Cell(row, 11).Value = student.CircleName;
            worksheet.Cell(row, 12).Value = student.IsSpecial;
            worksheet.Cell(row, 13).Value = student.WomanActivityType;
            worksheet.Cell(row, 14).Value = student.LearnCertificate;
            worksheet.Cell(row, 15).Value = student.ThePassword;
            worksheet.Cell(row, 16).Value = student.LeaveCount;
            worksheet.Cell(row, 17).Value = student.CompleteFollowup;
            worksheet.Cell(row, 18).Value = student.IsElite ? "نعم" : "لا";
            row++;
        }

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
