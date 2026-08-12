namespace AdminAPI.Configuration;

public class DeploymentOptions
{
    public const string SectionName = "Deployment";

    /// <summary>When true, EnsureCreated + seed default MasgedSettings row on startup. Column/table patches always run.</summary>
    public bool EnsureDatabase { get; set; }
}
