using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

/// <summary>
/// Persists the last successful age-update date so the job runs at most once per Kuwait calendar day.
/// </summary>
public class AgeUpdateLastRunState
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public AgeUpdateLastRunState(IOptions<AgeUpdateOptions> options)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "Logs");
        Directory.CreateDirectory(directory);
        _filePath = Path.Combine(directory, options.Value.StateFileName);
    }

    public bool HasRunToday()
    {
        var lastRun = ReadLastRunDate();
        return lastRun == KuwaitTime.Today;
    }

    public void MarkRunToday()
    {
        lock (_lock)
        {
            File.WriteAllText(_filePath, KuwaitTime.Today.ToString("yyyy-MM-dd"));
        }
    }

    private DateTime? ReadLastRunDate()
    {
        lock (_lock)
        {
            if (!File.Exists(_filePath))
                return null;

            var text = File.ReadAllText(_filePath).Trim();
            return DateTime.TryParseExact(
                text,
                "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var date)
                ? date.Date
                : null;
        }
    }
}
