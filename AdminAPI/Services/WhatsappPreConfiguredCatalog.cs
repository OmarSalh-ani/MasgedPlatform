namespace AdminAPI.Services;

public static class WhatsappPreConfiguredCatalog
{
    public static readonly string[] EventKeys =
    [
        "StudentAttendance",
        "StudentAbsence",
        "StudentMarkAsSpecial",
        "StudentMarkedAsElite",
        "StudentRevise",
        "StudentTest",
        "RegistrationNewSubmission",
        "RegistrationParentFollowupLink",
        "ParentPortalWelcome",
        "GoogleMeetCreated",
    ];

    public static string GetDefaultMessage(string eventName, string masgedName) => eventName switch
    {
        "StudentAttendance" =>
            "تم تسجيل حضور الطالب\n{اسم الطالب}\n\n{التاريخ}\n{الوقت}\n\n{اسم الحلقة}",
        "StudentAbsence" =>
            "تم تسجيل انصراف الطالب\n{اسم الطالب}\n\n{التاريخ}\n{الوقت}\n\n{اسم الحلقة}",
        "StudentMarkAsSpecial" =>
            "تهانينا! تم تعيين الطالب {اسم الطالب} كطالب مميز في حلقة {اسم الحلقة}",
        "StudentMarkedAsElite" =>
            "تهانينا! تم تعيين الطالب {اسم الطالب} كطالب نخبة في حلقة {اسم الحلقة}",
        "StudentRevise" =>
            "تم تسجيل {نوع المراجعة} للطالب {اسم الطالب}\n\nاسم السورة: {اسم السورة}\nمن: {من}\nإلى: {إلى}\n\n{التاريخ}\n{اسم الحلقة}",
        "StudentTest" =>
            "تم إضافة اختبار للطالب {اسم الطالب}\n\nتاريخ الاختبار: {تاريخ الاختبار}\nاسم السورة: {اسم السورة}\nالمجموع: {المجموع}\nالتقدير: {التقدير}\n\n{اسم الحلقة}",
        "RegistrationNewSubmission" =>
            $"تسجيل جديد في حلقات {masgedName}\n\nاسم الطالب: {{اسم الطالب}}\nالعمر: {{العمر}}\nجوال ولي الأمر: {{جوال ولي الأمر}}\nجوال ولي الأمر البديل: {{جوال ولي الأمر البديل}}\n{{نوع النشاط}}\nالتاريخ: {{التاريخ}}\nالوقت: {{الوقت}}",
        "RegistrationParentFollowupLink" =>
            "السلام عليكم ورحمة الله وبركاته\n\nنتمنى منكم الدخول لإكمال بيانات تسجيل الطالب {اسم الطالب}.\n📌 رابط التسجيل: {رابط المتابعة}",
        "ParentPortalWelcome" =>
            "تم استكمال تسجيل الطالب {اسم الطالب}.\nيمكنكم متابعة حضور وانصراف الطالب ونتائجه عبر بوابة ولي الأمر:\n{رابط بوابة ولي الأمر}\n\nاسم المستخدم: {رقم جوال ولي الأمر}\nكلمة المرور: {رقم جوال ولي الأمر}\nرابط شرح بالفيديو: {رابط الفيديو}",
        "GoogleMeetCreated" =>
            "السلام عليكم ورحمة الله وبركاته\n\nيوجد اجتماع جديد قادم للطالب {اسم الطالب} مع المعلم {اسم المعلم}\n\nعنوان الاجتماع: {اسم الاجتماع}\nالتاريخ: {التاريخ}\nالوقت: {الوقت}\n\nرابط الاجتماع:\n{رابط الاجتماع}",
        _ => "رسالة افتراضية",
    };

    public static string GetDisplayName(string eventName) => eventName switch
    {
        "StudentAttendance" => "حضور الطالب",
        "StudentAbsence" => "انصراف الطالب",
        "StudentMarkAsSpecial" => "تعيين طالب مميز",
        "StudentMarkedAsElite" => "تعيين طالب نخبة",
        "StudentRevise" => "مراجعة الطالب",
        "StudentTest" => "اختبار الطالب",
        "RegistrationNewSubmission" => "تسجيل جديد (إشعار الإدارة)",
        "RegistrationParentFollowupLink" => "رابط استكمال بيانات التسجيل لولي الأمر",
        "ParentPortalWelcome" => "ترحيب وتفاصيل بوابة ولي الأمر",
        "GoogleMeetCreated" => "إنشاء رابط Google Meet",
        _ => eventName,
    };

    public static string GetDescription(string eventName) => eventName switch
    {
        "StudentAttendance" => "يتم إرسال هذه الرسالة عند تسجيل حضور الطالب",
        "StudentAbsence" => "يتم إرسال هذه الرسالة عند تسجيل انصراف الطالب",
        "StudentMarkAsSpecial" => "يتم إرسال هذه الرسالة عند تعيين الطالب كطالب مميز",
        "StudentMarkedAsElite" => "يتم إرسال هذه الرسالة عند تعيين الطالب كطالب نخبة",
        "StudentRevise" => "يتم إرسال هذه الرسالة عند حفظ أو مراجعة الطالب",
        "StudentTest" => "يتم إرسال هذه الرسالة عند إضافة اختبار للطالب",
        "RegistrationNewSubmission" => "يتم إرسال هذه الرسالة لإشعار الإدارة بطلب تسجيل جديد",
        "RegistrationParentFollowupLink" => "يتم إرسال هذه الرسالة إلى ولي الأمر لإكمال بيانات التسجيل",
        "ParentPortalWelcome" => "يتم إرسال هذه الرسالة بعد استكمال التسجيل لشرح بوابة ولي الأمر",
        "GoogleMeetCreated" => "يتم إرسال هذه الرسالة عند إنشاء المعلم لرابط Google Meet للطلاب المحددين",
        _ => "حدث غير محدد",
    };
}
