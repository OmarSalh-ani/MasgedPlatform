namespace AdminAPI.Configuration;

public class DeploymentOptions
{
    public const string SectionName = "Deployment";

    /// <summary>Apex domain from compose (.env DOMAIN). Used for SSL hostnames and setup defaults.</summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>When true, EnsureCreated + white-label column ensure on startup (Docker).</summary>
    public bool EnsureDatabase { get; set; }
}
