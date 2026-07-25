using System.Net;
using System.Text;
using MasgedTeacherMobileAPI.Dtos;

namespace MasgedTeacherMobileAPI.Helpers;

public static class TestCertificateHtmlBuilder
{
    public static string Build(TestCertificateDto data, string? logoBaseUrl = null)
    {
        static string Enc(string? value) => WebUtility.HtmlEncode(value ?? "");
        var hizbCells = data.HizbCells.Count >= 8
            ? data.HizbCells.Take(8).ToList()
            : PadHizbCells(data.HizbCells);

        var hizbHtml = new StringBuilder();
        foreach (var cell in hizbCells)
            hizbHtml.Append("<div class=\"hizb-cell\">").Append(Enc(cell)).Append("</div>");

        var logoHtml = BuildLogoHtml(logoBaseUrl);

        return $@"<!DOCTYPE html>
<html lang=""ar"" dir=""rtl"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>شهادة اختبار طالب</title>
<link href=""https://fonts.googleapis.com/css2?family=Amiri:wght@400;700&family=Tajawal:wght@400;500;700&display=swap"" rel=""stylesheet"">
<style>
:root {{ --primary-color: #438BB6; --secondary-color: #357194; --accent-color: #c5a059; --bg-light: #fdfbf7; --text-dark: #2d3436; --border-color: #d1b88a; }}
@page {{ size: A4 landscape; margin: 0 !important; }}
body {{ margin: 0; padding: 0; font-family: 'Tajawal', sans-serif; background-color: #e9ecef; display: flex; justify-content: center; align-items: center; min-height: 100vh; color: var(--text-dark); -webkit-print-color-adjust: exact; print-color-adjust: exact; }}
.certificate-wrapper {{ width: 297mm; height: 210mm; background-color: var(--bg-light) !important; padding: 8mm; box-sizing: border-box; position: relative; box-shadow: 0 20px 50px rgba(0,0,0,0.15); overflow: hidden; display: flex; flex-direction: column; }}
.islamic-pattern {{ position: absolute; inset: 0; opacity: 0.06; background-image: url(""data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M30 0l15 15-15 15-15-15zM0 30l15 15-15 15-15-15zM60 30l15 15-15 15-15-15zM30 60l15 15-15 15-15-15z' fill='%23438BB6' fill-rule='evenodd'/%3E%3C/svg%3E""); z-index: 0; }}
.outer-border {{ position: absolute; top: 8mm; left: 8mm; right: 8mm; bottom: 8mm; border: 4px double var(--accent-color); z-index: 1; pointer-events: none; }}
.inner-border {{ position: absolute; top: 12mm; left: 12mm; right: 12mm; bottom: 12mm; border: 1px solid var(--accent-color); z-index: 1; pointer-events: none; }}
.corner {{ position: absolute; width: 60px; height: 60px; border: 3px solid var(--accent-color); z-index: 2; }}
.top-right {{ top: 12mm; right: 12mm; border-left: none; border-bottom: none; border-radius: 0 15px 0 0; }}
.top-left {{ top: 12mm; left: 12mm; border-right: none; border-bottom: none; border-radius: 15px 0 0 0; }}
.bottom-right {{ bottom: 12mm; right: 12mm; border-left: none; border-top: none; border-radius: 0 0 15px 0; }}
.bottom-left {{ bottom: 12mm; left: 12mm; border-right: none; border-top: none; border-radius: 0 0 0 15px; }}
.content-inner {{ position: relative; z-index: 3; height: 100%; display: flex; flex-direction: column; padding: 30px 60px; box-sizing: border-box; }}
.header {{ display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }}
.logo-section {{ width: 220px; text-align: center; }}
.logo-img {{ width: 150px; height: 120px; margin: 0 auto 10px; display: block; object-fit: contain; }}
.logo-placeholder {{ width: 60px; height: 60px; margin: 0 auto 10px; background: var(--primary-color) !important; clip-path: polygon(50% 0%, 61% 35%, 98% 35%, 68% 57%, 79% 91%, 50% 70%, 21% 91%, 32% 57%, 2% 35%, 39% 35%); }}
.title-section {{ text-align: center; flex-grow: 1; }}
.title-section h1 {{ font-family: 'Amiri', serif; font-size: 48px; color: var(--primary-color); margin: 0; padding-bottom: 10px; border-bottom: 2px solid var(--accent-color); display: inline-block; }}
.main-body {{ margin-top: 20px; text-align: center; flex-grow: 1; display: flex; flex-direction: column; justify-content: space-around; }}
.intro-text {{ font-size: 20px; color: #444; }}
.student-highlight {{ margin: 15px 0; }}
.name-label {{ font-size: 18px; color: #666; }}
.name-value {{ font-family: 'Amiri', serif; font-size: 36px; font-weight: 700; color: var(--primary-color); border-bottom: 1px dashed var(--accent-color); padding: 0 30px; }}
.details-row {{ display: flex; justify-content: space-between; align-items: flex-start; gap: 40px; margin-top: 20px; }}
.hizb-container {{ flex: 1; text-align: right; }}
.hizb-label {{ font-weight: 700; color: var(--primary-color); margin-bottom: 10px; display: block; }}
.hizb-grid {{ display: grid; grid-template-columns: repeat(8, 1fr); gap: 5px; max-width: 350px; }}
.hizb-cell {{ aspect-ratio: 1; border: 1px solid var(--accent-color); background: white !important; border-radius: 4px; display: flex; align-items: center; justify-content: center; font-weight: bold; }}
.grades-container {{ flex: 2; }}
.grades-table {{ width: 100%; border-collapse: collapse; background: white !important; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }}
.grades-table th {{ background: var(--primary-color) !important; color: white !important; padding: 12px; font-size: 16px; border: 1px solid var(--secondary-color); }}
.grades-table td {{ padding: 15px; border: 1px solid var(--border-color); height: 30px; text-align: center; }}
.footer {{ display: flex; justify-content: space-between; align-items: flex-end; margin-top: 20px; padding: 0 20px; }}
.date-info {{ font-size: 16px; color: var(--primary-color); font-weight: 500; }}
.signature-box {{ text-align: center; width: 250px; font-weight: 700; font-size: 14px; }}
.sig-line {{ border-top: 1px solid var(--accent-color); margin-top: 3px; padding-top: 8px; }}
.no-print-zone {{ position: fixed; top: 20px; left: 20px; z-index: 1000; }}
.btn {{ padding: 10px 25px; background: var(--primary-color); color: white; border: none; border-radius: 4px; cursor: pointer; font-family: 'Tajawal', sans-serif; font-weight: 700; }}
@media print {{ .no-print-zone {{ display: none !important; }} body {{ background: white; }} .certificate-wrapper {{ box-shadow: none; margin: 0; }} }}
</style>
</head>
<body>
<div class=""no-print-zone""><button class=""btn"" onclick=""window.print()"">طباعة الشهادة</button></div>
<div class=""certificate-wrapper"">
<div class=""islamic-pattern""></div>
<div class=""outer-border""></div><div class=""inner-border""></div>
<div class=""corner top-right""></div><div class=""corner top-left""></div><div class=""corner bottom-right""></div><div class=""corner bottom-left""></div>
<div class=""content-inner"">
<header class=""header"">
<div class=""logo-section"">{logoHtml}</div>
<div class=""title-section""><h1>شهادة اختبار طالب</h1></div>
<div style=""width:220px;""></div>
</header>
<main class=""main-body"">
<div class=""intro-text"">تشهد إدارة حلقات <strong>مسجد الشيخ مبارك عبدالله المبارك الصباح</strong></div>
<div class=""student-highlight"">
<span class=""name-label"">بأن الطالب:</span><br>
<span class=""name-value"">{Enc(data.StudentName)}</span>
</div>
<div class=""intro-text"">والمسجل بحلقة: <strong>{Enc(data.CircleName)}</strong> قد تقدم للاختبارات <strong>{Enc(data.TestPeriod)}</strong></div>
<div class=""details-row"">
<div class=""hizb-container"">
<span class=""hizb-label"">الأحزاب :</span>
<div class=""hizb-grid"">
{hizbHtml}
</div>
</div>
<div class=""grades-container"">
<table class=""grades-table"">
<thead><tr><th>الحفظ 70</th><th>التجويد 20</th><th>الأداء 10</th><th>المجموع 100</th><th>التقدير</th></tr></thead>
<tbody><tr>
<td>{Enc(data.MemorizationScore)}</td>
<td>{Enc(data.TajweedScore)}</td>
<td>{Enc(data.RevisionScore)}</td>
<td>{Enc(data.TotalScore)}</td>
<td>{Enc(data.Grade)}</td>
</tr></tbody>
</table>
</div>
</div>
</div>
</main>
<footer class=""footer"">
<div class=""date-info"">تاريخ الاختبار: {Enc(data.TestDate)}</div>
<div class=""signature-box"">مدير إدارة مساجد محافظة الجهراء<div class=""sig-line""></div></div>
</footer>
</div>
</div>
</body>
</html>";
    }

    private static List<string> PadHizbCells(List<string> cells)
    {
        var result = new List<string>(8);
        for (var i = 0; i < 8; i++)
            result.Add(i < cells.Count ? cells[i] : "");
        return result;
    }

    private static string BuildLogoHtml(string? logoBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(logoBaseUrl))
            return "<div class=\"logo-placeholder\"></div>";

        var baseUrl = logoBaseUrl.TrimEnd('/');
        var logo1 = WebUtility.HtmlEncode($"{baseUrl}/Logo1.png");
        var logo2 = WebUtility.HtmlEncode($"{baseUrl}/logoo2.jpeg");
        return $@"<div class=""logo-dual"" style=""display:flex;align-items:center;justify-content:center;gap:10px;"">
<img src=""{logo1}"" alt=""شعار"" class=""logo-img"" />
<img src=""{logo2}"" alt=""شعار"" class=""logo-img"" />
</div>";
    }
}
