namespace MasgedTeacherMobileAPI.Helpers;

public static class KuwaitTime
{
    private static readonly TimeZoneInfo KuwaitTz =
        TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, KuwaitTz);

    public static DateTime Today => Now.Date;

    public static readonly TimeSpan Offset = TimeSpan.FromHours(3);

    /// <summary>Kuwait-local [DateTime] as offset (+03:00) for API/SignalR JSON.</summary>
    public static DateTimeOffset ToOffset(DateTime kuwaitLocal) =>
        new(DateTime.SpecifyKind(kuwaitLocal, DateTimeKind.Unspecified), Offset);

    public static DateTimeOffset NowOffset => ToOffset(Now);

    /// 23:59 on the given calendar day (Kuwait local date).
    public static DateTime EndOfDay(DateTime date) => date.Date.AddHours(23).AddMinutes(59);
}
