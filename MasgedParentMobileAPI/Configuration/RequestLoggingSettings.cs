namespace MasgedParentMobileAPI.Configuration;

public sealed class RequestLoggingSettings
{
    /// <summary>When false, the middleware skips all work.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Max characters stored per body (truncation suffix appended if exceeded).</summary>
    public int MaxBodyLength { get; set; } = 32_768;

    /// <summary>Request path prefixes to skip (e.g. swagger, hubs).</summary>
    public string[] ExcludedPaths { get; set; } = ["/swagger", "/hubs/"];
}
