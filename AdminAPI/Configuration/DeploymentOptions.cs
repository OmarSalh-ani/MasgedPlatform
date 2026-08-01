namespace AdminAPI.Configuration;

public class DeploymentOptions
{
    public const string SectionName = "Deployment";

  /// <summary>Apex domain for this deployment. Prefills the domain field in the setup wizard; the value can be changed there.</summary>
  public string Domain { get; set; } = string.Empty;

  /// <summary>When true, EnsureCreated + white-label column ensure on startup.</summary>
  public bool EnsureDatabase { get; set; }
}
