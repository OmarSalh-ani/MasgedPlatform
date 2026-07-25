namespace MasgedTeacherMobileAPI.Helpers;

public static class PlanRowStatus
{
    public const string Pass = "تم الحفظ";
    public const string Fail = "لم يتم الحفظ";
    public const string Pending = "منتظر التسميع";
    public const string Retake = "اعادة تسميع";

    public static readonly string[] ValidStatuses =
    [
        Pass, Fail, Pending, Retake
    ];

    public static readonly string[] PendingStatuses =
    [
        Pending, "قيد الانتظار"
    ];

    public static readonly string[] CompletedStatuses =
    [
        Pass, "تم", "قيد الانتظار في التثبيت"
    ];

    public static string Normalize(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return Pending;

        return status switch
        {
            "تم" or "قيد الانتظار في التثبيت" => Pass,
            "لم يتم" => Fail,
            "قيد الانتظار" => Pending,
            _ => status
        };
    }

    public static bool IsPass(string? status) => Normalize(status) == Pass;

    public static bool IsPending(string? status) => Normalize(status) == Pending;

    public static bool IsFail(string? status) => Normalize(status) == Fail;

    public static bool IsRetake(string? status) => Normalize(status) == Retake;

    public static bool IsCompletedLogStatus(string? status) =>
        CompletedStatuses.Contains(status);

    public static string GetDisplayLabel(string rowKey, string? status)
    {
        var normalized = Normalize(status);
        if (string.IsNullOrEmpty(normalized))
            return "";

        if (rowKey.StartsWith("memorizing_"))
            return normalized;

        if (rowKey.StartsWith("revise_"))
            return normalized switch
            {
                Pass => "تم المراجعة",
                Fail => "لم يتم المراجعة",
                Retake => "اعادة مراجعة",
                _ => normalized
            };

        return normalized switch
        {
            Pass => "تم التثبيت",
            Fail => "لم يتم التثبيت",
            _ => normalized
        };
    }
}
