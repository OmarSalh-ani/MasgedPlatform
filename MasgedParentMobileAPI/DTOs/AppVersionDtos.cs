namespace MasgedParentMobileAPI.DTOs;

public sealed class AppVersionResponseDto
{
    public string MinimumVersion { get; set; } = "1.0.0";

    public int MinimumBuildNumber { get; set; }

    public string? UpdateMessage { get; set; }

    public string? GooglePlayUrl { get; set; }

    public string? AppStoreUrl { get; set; }
}
