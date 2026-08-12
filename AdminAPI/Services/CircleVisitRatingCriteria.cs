namespace AdminAPI.Services;

public static class CircleVisitRatingCriteria
{
    public static readonly string[] Criteria =
    [
        "حضور المحفظ",
        "انضباط وقت الحلقة",
        "عدد الطلاب الحاضرين",
        "مستوى حفظ الطلاب",
        "المراجعة اليومية",
        "السلوك والانضباط",
        "سجل الحضور",
        "متابعة أولياء الامور",
        "البيئة التعليمية",
    ];

    public static readonly string[] Ratings =
    [
        "ممتاز",
        "جيد جدا",
        "جيد",
        "يحتاج متابعة",
    ];
}
