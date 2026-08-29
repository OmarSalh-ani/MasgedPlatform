using MasgedTeacherMobileAPI.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MasgedTeacherMobileAPI.Helpers;

public static class TestCertificatePdfExporter
{
    private static readonly Color PrimaryBlue = Color.FromRGB(67, 139, 182);
    private static readonly Color AccentGold = Color.FromRGB(197, 160, 89);
    private static readonly Color BodyText = Color.FromRGB(45, 52, 54);
    private static readonly Color MutedText = Color.FromRGB(102, 102, 102);
    private static readonly Color BorderColor = Color.FromRGB(209, 184, 138);
    private static readonly Color Background = Color.FromRGB(253, 251, 247);

    public static byte[] Build(TestCertificateDto data)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        CircleMemorizationRevisionReportAssets.EnsureFontsRegistered();

        var logoBytes = CircleMemorizationRevisionReportAssets.TryReadLogoBytes();
        var fontRegular = CircleMemorizationRevisionReportAssets.FontRegular;
        var fontBold = CircleMemorizationRevisionReportAssets.FontBold;
        var hizbCells = PadHizbCells(data.HizbCells);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(28);
                page.DefaultTextStyle(x => x
                    .FontSize(11)
                    .FontFamily(fontRegular)
                    .FontColor(BodyText));
                page.ContentFromRightToLeft();
                page.PageColor(Background);

                page.Content().Border(2).BorderColor(AccentGold).Padding(18).Column(col =>
                {
                    col.Item().Border(1).BorderColor(AccentGold).Padding(14).Column(inner =>
                    {
                        inner.Item().Row(row =>
                        {
                            if (logoBytes is { Length: > 0 })
                                row.ConstantItem(72).Height(56).Image(logoBytes).FitArea();

                            row.RelativeItem().AlignCenter().Text("شهادة اختبار طالب")
                                .FontFamily(fontBold).Bold().FontSize(28).FontColor(PrimaryBlue);

                            row.ConstantItem(72);
                        });

                        inner.Item().PaddingTop(14).AlignCenter()
                            .Text("تشهد إدارة حلقات مسجد الشيخ مبارك عبدالله المبارك الصباح")
                            .FontFamily(fontRegular).FontSize(13).FontColor(MutedText);

                        inner.Item().PaddingTop(10).AlignCenter().Text("بأن الطالب:")
                            .FontFamily(fontRegular).FontSize(12).FontColor(MutedText);
                        inner.Item().PaddingTop(4).AlignCenter().Text(data.StudentName)
                            .FontFamily(fontBold).Bold().FontSize(24).FontColor(PrimaryBlue);

                        inner.Item().PaddingTop(10).AlignCenter().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontFamily(fontRegular).FontSize(13));
                            text.Span("والمسجل بحلقة: ");
                            text.Span(data.CircleName).Bold().FontFamily(fontBold);
                            text.Span(" قد تقدم للاختبارات ");
                            text.Span(data.TestPeriod).Bold().FontFamily(fontBold);
                        });

                        inner.Item().PaddingTop(18).Row(row =>
                        {
                            row.RelativeItem(2).Column(hizbCol =>
                            {
                                hizbCol.Item().Text("الأحزاب:")
                                    .FontFamily(fontBold).Bold().FontSize(12).FontColor(PrimaryBlue);
                                hizbCol.Item().PaddingTop(8).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        for (var i = 0; i < 8; i++)
                                            columns.RelativeColumn();
                                    });

                                    foreach (var cell in hizbCells)
                                    {
                                        table.Cell().Element(HizbCellStyle)
                                            .Text(cell)
                                            .FontFamily(fontBold).Bold().FontSize(10);
                                    }
                                });
                            });

                            row.RelativeItem(3).PaddingRight(12).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                foreach (var title in new[]
                                         {
                                             "الحفظ 70", "التجويد 20", "الأداء 10", "المجموع 100", "التقدير"
                                         })
                                {
                                    table.Cell().Element(HeaderCellStyle)
                                        .Text(title)
                                        .FontFamily(fontBold).Bold().FontSize(10).FontColor(Colors.White);
                                }

                                table.Cell().Element(BodyCellStyle).Text(data.MemorizationScore);
                                table.Cell().Element(BodyCellStyle).Text(data.TajweedScore);
                                table.Cell().Element(BodyCellStyle).Text(data.RevisionScore);
                                table.Cell().Element(BodyCellStyle).Text(data.TotalScore);
                                table.Cell().Element(BodyCellStyle).Text(data.Grade)
                                    .FontFamily(fontBold).Bold();
                            });
                        });

                        inner.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text("تاريخ الاختبار: " + data.TestDate)
                                .FontFamily(fontRegular).FontSize(11).FontColor(PrimaryBlue);
                            row.RelativeItem().AlignRight().Column(sig =>
                            {
                                sig.Item().AlignCenter().Text("مدير إدارة مساجد محافظة الجهراء")
                                    .FontFamily(fontBold).Bold().FontSize(11);
                                sig.Item().PaddingTop(8).LineHorizontal(1).LineColor(AccentGold);
                            });
                        });
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    private static List<string> PadHizbCells(IReadOnlyList<string> cells)
    {
        var result = new List<string>(8);
        for (var i = 0; i < 8; i++)
            result.Add(i < cells.Count ? cells[i] : string.Empty);
        return result;
    }

    private static IContainer HeaderCellStyle(IContainer container) =>
        container
            .Background(PrimaryBlue)
            .Border(0.75f)
            .BorderColor(PrimaryBlue)
            .PaddingVertical(8)
            .PaddingHorizontal(4)
            .AlignCenter()
            .AlignMiddle();

    private static IContainer BodyCellStyle(IContainer container) =>
        container
            .Background(Colors.White)
            .Border(0.75f)
            .BorderColor(BorderColor)
            .PaddingVertical(10)
            .PaddingHorizontal(4)
            .AlignCenter()
            .AlignMiddle();

    private static IContainer HizbCellStyle(IContainer container) =>
        container
            .Background(Colors.White)
            .Border(0.75f)
            .BorderColor(BorderColor)
            .PaddingVertical(8)
            .PaddingHorizontal(2)
            .AlignCenter()
            .AlignMiddle();
}
