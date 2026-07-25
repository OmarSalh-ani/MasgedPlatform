namespace AdminAPI.Services;

public static class WhatsappPreConfiguredPreview
{
    public static string ApplySampleData(string message)
    {
        return (message ?? string.Empty)
            .Replace("{رقم الطالب}", "12345")
            .Replace("{اسم الطالب}", "أحمد محمد")
            .Replace("{اسم الأب}", "محمد أحمد")
            .Replace("{التاريخ}", KuwaitTime.Now.ToString("dd-MM-yyyy"))
            .Replace("{الوقت}", KuwaitTime.Now.ToString("hh:mm tt"))
            .Replace("{اسم الحلقة}", "حلقة الفجر")
            .Replace("{اسم المعلم}", "الشيخ عبدالله")
            .Replace("{نوع المراجعة}", "حفظ")
            .Replace("{اسم السورة}", "البقرة")
            .Replace("{من}", "1")
            .Replace("{إلى}", "10")
            .Replace("{ملاحظات}", "ملاحظات تجريبية")
            .Replace("{تاريخ الاختبار}", KuwaitTime.Now.ToString("dd-MM-yyyy"))
            .Replace("{حزب رقم}", "1, 2")
            .Replace("{درجة الحفظ}", "70")
            .Replace("{درجة التجويد}", "20")
            .Replace("{درجة الأداء}", "10")
            .Replace("{المجموع}", "100")
            .Replace("{التقدير}", "ممتاز")
            .Replace("{النتيجة النهائية}", "95")
            .Replace("{العمر}", "12")
            .Replace("{جوال ولي الأمر}", "60000000")
            .Replace("{جوال ولي الأمر البديل}", "60000001")
            .Replace("{نوع النشاط}", "نشاط نسائي")
            .Replace("{رابط المتابعة}", "https://mosque-mbark-j.com/ParentsFollowup.aspx?id=123")
            .Replace("{اسم الاجتماع}", "اجتماع دار القرآن التفاعلي")
            .Replace("{رابط الاجتماع}", "https://meet.google.com/abc-defg-hij");
    }
}
