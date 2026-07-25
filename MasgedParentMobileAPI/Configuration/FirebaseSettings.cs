namespace MasgedParentMobileAPI.Configuration;

public sealed class FirebaseSettings
{
    public const string SectionName = "Firebase";

    /// <summary>When false, push sending is skipped (WhatsApp still works).</summary>
    public bool Enabled { get; set; }

    public string ProjectId { get; set; } = string.Empty;

    /// <summary>Absolute path to Firebase service account JSON on the server.</summary>
    public string ServiceAccountJsonPath { get; set; } = string.Empty;
}
