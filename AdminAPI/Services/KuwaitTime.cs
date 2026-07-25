namespace AdminAPI.Services;

public static class KuwaitTime
{
    private static readonly TimeZoneInfo KuwaitZone = ResolveKuwaitZone();

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, KuwaitZone);

    public static DateTime Today => Now.Date;

    private static TimeZoneInfo ResolveKuwaitZone()
    {
        foreach (var id in new[] { "Asia/Kuwait", "Arab Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Utc;
    }
}
