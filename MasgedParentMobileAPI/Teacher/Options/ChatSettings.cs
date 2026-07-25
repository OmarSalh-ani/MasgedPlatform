namespace MasgedTeacherMobileAPI.Options;

public sealed class ChatSettings
{
    public const string SectionName = "Chat";

    /// <summary>Parent Mobile API origin (scheme + host + optional port).</summary>
    public string ParentApiBaseUrl { get; set; } = string.Empty;

    /// <summary>Shared secret matching Parent API Chat:InternalBroadcastKey.</summary>
    public string InternalBroadcastKey { get; set; } = string.Empty;
}
