namespace MasgedParentMobileAPI.Models;

public sealed class ParentDeviceToken
{
    public int Id { get; set; }

    /// <summary>Canonical Kuwait phone (965XXXXXXXX).</summary>
    public string ParentPhone { get; set; } = string.Empty;

    public string FcmToken { get; set; } = string.Empty;

    public string Platform { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}
