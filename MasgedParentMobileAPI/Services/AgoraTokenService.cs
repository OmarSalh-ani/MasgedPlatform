using System.Globalization;
using AgoraIO.Media;
using Microsoft.Extensions.Options;

namespace MasgedParentMobileAPI.Services;

public sealed class AgoraOptions
{
    public const string SectionName = "Agora";

    public string AppId { get; set; } = string.Empty;

    /// <summary>Agora primary (or secondary) App Certificate from console. Keep on server only.</summary>
    public string AppCertificate { get; set; } = string.Empty;
}

/// <summary>RTC tokens for Agora SDK 4.x (UID-scoped publisher privileges).</summary>
public sealed class AgoraTokenService(
    IOptions<AgoraOptions> options,
    AgoraSecretsCache secretsCache)
{
    /// <summary>Builds a publisher token valid for ~1 hour.</summary>
    public string BuildRtcPublisherToken(string channelName, uint uid)
    {
        var appId = FirstNonEmpty(secretsCache.AppId, options.Value.AppId);
        var appCertificate = FirstNonEmpty(secretsCache.AppCertificate, options.Value.AppCertificate);

        if (string.IsNullOrWhiteSpace(appId))
            throw new InvalidOperationException("Configure Agora:AppId.");
        if (string.IsNullOrWhiteSpace(appCertificate))
            throw new InvalidOperationException("Configure Agora:AppCertificate (server-side secret).");

        var privilegeExpiredTs =
            (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600);

        var token = new AccessToken(
            appId,
            appCertificate,
            channelName,
            uid.ToString(CultureInfo.InvariantCulture));

        token.AddPrivilege(Privileges.kJoinChannel, privilegeExpiredTs);
        token.AddPrivilege(Privileges.kPublishAudioStream, privilegeExpiredTs);
        token.AddPrivilege(Privileges.kPublishVideoStream, privilegeExpiredTs);

        return token.Build();
    }

    private static string? FirstNonEmpty(string? preferred, string? fallback) =>
        !string.IsNullOrWhiteSpace(preferred) ? preferred.Trim() : (string.IsNullOrWhiteSpace(fallback) ? null : fallback.Trim());
}

/// <summary>Agora UID rules: teacher uses DB teacher id; parent device uses 1_000_000_000 + studentId.</summary>
public static class VideoCallUidRules
{
    public const uint ParentUidOffset = 1_000_000_000;

    public static uint TeacherUid(int teacherId) => (uint)teacherId;

    /// <summary>Secondary connection UID for teacher screen share (same channel).</summary>
    public static uint TeacherScreenUid(int teacherId) => (uint)teacherId + 1;

    public static uint ParentUid(int studentId) => ParentUidOffset + (uint)studentId;

    /// <summary>True for teacher camera or screen-share RTC uid.</summary>
    public static bool IsTeacherRtcUid(int rtcUid) =>
        rtcUid > 0 && rtcUid < (int)ParentUidOffset;

    /// <summary>If <paramref name="rtcUid"/> is a parent UID, returns student id; otherwise null.</summary>
    public static int? TryGetStudentIdFromRemoteUid(int rtcUid)
    {
        if (rtcUid <= 0)
            return null;
        var u = (uint)rtcUid;
        if (u < ParentUidOffset)
            return null;
        return (int)(u - ParentUidOffset);
    }
}
