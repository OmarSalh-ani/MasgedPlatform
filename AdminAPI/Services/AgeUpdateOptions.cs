namespace AdminAPI.Services;

public class AgeUpdateOptions
{
    public const string SectionName = "AgeUpdate";

    /// <summary>Hour (0–23) in Kuwait local time when the job should run.</summary>
    public int RunAtHour { get; set; } = 0;

    /// <summary>Minute (0–59) in Kuwait local time when the job should run.</summary>
    public int RunAtMinute { get; set; } = 0;

    public string StateFileName { get; set; } = "age_update_last_run.txt";
}
