using System.Globalization;

namespace AdminAPI.Services;

public static class GeorgianDateFormatter
{
    public static string FormatDate(DateTime? date)
    {
        if (!date.HasValue)
            return string.Empty;

        return date.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }

    public static string FormatDateTime(DateTime? date)
    {
        if (!date.HasValue)
            return string.Empty;

        return date.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
    }
}
