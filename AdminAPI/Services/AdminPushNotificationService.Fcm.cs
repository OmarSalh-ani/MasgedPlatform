using AdminAPI.Data;
using AdminAPI.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public sealed partial class AdminPushNotificationService
{
    private static readonly object FirebaseInitLock = new();
    private static bool _firebaseInitialized;

    private enum DeviceTokenKind
    {
        None,
        Parent,
        Teacher,
    }

    private async Task<(int SuccessCount, int FailureCount)> SendAdminMulticastAsync(
        IReadOnlyList<string> tokens,
        string title,
        string body,
        DeviceTokenKind tokenKind,
        CancellationToken cancellationToken)
    {
        const string context = "admin broadcast";

        if (!_firebaseSettings.Enabled)
        {
            await PersistSkipAsync(context, tokenKind, "PushDisabled",
                "FirebaseSettings.Enabled is false", title, body, cancellationToken);
            return (0, tokens.Count);
        }

        if (!EnsureInitialized())
        {
            await PersistSkipAsync(context, tokenKind, "FirebaseNotInitialized",
                "Firebase Admin SDK failed to initialize (check service account path)", title, body, cancellationToken);
            return (0, tokens.Count);
        }

        if (tokens.Count == 0)
        {
            await PersistSkipAsync(context, tokenKind, "NoTokens",
                "No device tokens for selected audience", title, body, cancellationToken);
            return (0, 0);
        }

        var data = new Dictionary<string, string>
        {
            ["kind"] = "admin",
            ["title"] = title,
            ["body"] = body,
        };

        var successCount = 0;
        var failureCount = 0;

        foreach (var batch in tokens.Chunk(500))
        {
            try
            {
                var message = new MulticastMessage
                {
                    Tokens = batch.ToList(),
                    Notification = new Notification { Title = title, Body = body },
                    Data = data,
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            ChannelId = "masged_admin",
                            ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                            Color = "#4A9B8F",
                            Icon = "ic_notification",
                        },
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps { Sound = "default", ContentAvailable = true },
                    },
                };

                var response = await FirebaseMessaging.DefaultInstance
                    .SendEachForMulticastAsync(message, cancellationToken);

                successCount += response.SuccessCount;
                failureCount += response.FailureCount;

                await LogAndPruneMulticastFailuresAsync(
                    response,
                    batch,
                    context,
                    tokenKind,
                    title,
                    body,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "FCM admin broadcast send failed");
                failureCount += batch.Length;
                await PersistSkipAsync(context, tokenKind, "SendException",
                    Truncate(ex.Message, 2000), title, body, cancellationToken);
            }
        }

        return (successCount, failureCount);
    }

    private async Task LogAndPruneMulticastFailuresAsync(
        BatchResponse response,
        IReadOnlyList<string> tokens,
        string context,
        DeviceTokenKind tokenKind,
        string? title,
        string? body,
        CancellationToken cancellationToken)
    {
        var metaByToken = await ResolveTokenMetaAsync(tokens, tokenKind, cancellationToken);
        var now = DateTime.UtcNow;
        var logs = new List<PushDeliveryLog>(tokens.Count);
        var staleTokens = new List<string>();

        for (var i = 0; i < response.Responses.Count; i++)
        {
            var sendResponse = response.Responses[i];
            var token = tokens[i];
            var platform = string.Empty;
            string? ownerKey = null;
            if (metaByToken.TryGetValue(token, out var meta))
            {
                platform = meta.Platform ?? string.Empty;
                ownerKey = meta.OwnerKey;
            }

            if (sendResponse.IsSuccess)
            {
                logs.Add(new PushDeliveryLog
                {
                    CreatedAt = now,
                    Source = "AdminAPI",
                    Context = Truncate(context, 200) ?? context,
                    AudienceKind = AudienceLabel(tokenKind),
                    Platform = platform,
                    OwnerKey = Truncate(ownerKey, 64),
                    FcmToken = Truncate(token, 512),
                    Success = true,
                    MessageId = Truncate(sendResponse.MessageId, 200),
                    Title = Truncate(title, 200),
                    BodyPreview = Truncate(body, 300),
                });
                continue;
            }

            var errorCode = sendResponse.Exception?.MessagingErrorCode;
            var errorCodeText = errorCode?.ToString() ?? "unknown";
            var detail = Truncate(sendResponse.Exception?.Message, 2000);

            logger.LogWarning(
                "FCM delivery failed ({Context}): error={ErrorCode} platform={Platform} detail={Detail}",
                context,
                errorCodeText,
                platform,
                detail);

            logs.Add(new PushDeliveryLog
            {
                CreatedAt = now,
                Source = "AdminAPI",
                Context = Truncate(context, 200) ?? context,
                AudienceKind = AudienceLabel(tokenKind),
                Platform = platform,
                OwnerKey = Truncate(ownerKey, 64),
                FcmToken = Truncate(token, 512),
                Success = false,
                ErrorCode = Truncate(errorCodeText, 100),
                ErrorDetail = detail,
                Title = Truncate(title, 200),
                BodyPreview = Truncate(body, 300),
            });

            if (ShouldPruneToken(errorCode))
                staleTokens.Add(token);
        }

        if (logs.Count > 0)
        {
            db.PushDeliveryLogs.AddRange(logs);
            await db.SaveChangesAsync(cancellationToken);
        }

        if (staleTokens.Count == 0 || tokenKind == DeviceTokenKind.None)
            return;

        if (tokenKind == DeviceTokenKind.Parent)
        {
            var rows = await db.ParentDeviceTokens
                .Where(t => staleTokens.Contains(t.FcmToken))
                .ToListAsync(cancellationToken);

            if (rows.Count > 0)
            {
                db.ParentDeviceTokens.RemoveRange(rows);
                await db.SaveChangesAsync(cancellationToken);
            }
        }
        else
        {
            var rows = await db.TeacherDeviceTokens
                .Where(t => staleTokens.Contains(t.FcmToken))
                .ToListAsync(cancellationToken);

            if (rows.Count > 0)
            {
                db.TeacherDeviceTokens.RemoveRange(rows);
                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task PersistSkipAsync(
        string context,
        DeviceTokenKind tokenKind,
        string errorCode,
        string? detail,
        string? title,
        string? body,
        CancellationToken cancellationToken)
    {
        try
        {
            db.PushDeliveryLogs.Add(new PushDeliveryLog
            {
                CreatedAt = DateTime.UtcNow,
                Source = "AdminAPI",
                Context = Truncate(context, 200) ?? context,
                AudienceKind = AudienceLabel(tokenKind),
                Platform = string.Empty,
                Success = false,
                ErrorCode = Truncate(errorCode, 100),
                ErrorDetail = Truncate(detail, 2000),
                Title = Truncate(title, 200),
                BodyPreview = Truncate(body, 300),
            });
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to persist push skip log for {Context}", context);
        }
    }

    private async Task<Dictionary<string, (string Platform, string? OwnerKey)>> ResolveTokenMetaAsync(
        IReadOnlyList<string> tokens,
        DeviceTokenKind tokenKind,
        CancellationToken cancellationToken)
    {
        if (tokens.Count == 0)
            return new Dictionary<string, (string, string?)>(StringComparer.Ordinal);

        if (tokenKind == DeviceTokenKind.Parent)
        {
            var rows = await db.ParentDeviceTokens
                .AsNoTracking()
                .Where(t => tokens.Contains(t.FcmToken))
                .Select(t => new { t.FcmToken, t.Platform, t.ParentPhone })
                .ToListAsync(cancellationToken);

            return rows
                .GroupBy(r => r.FcmToken, StringComparer.Ordinal)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var first = g.First();
                        return (first.Platform ?? string.Empty, (string?)first.ParentPhone);
                    },
                    StringComparer.Ordinal);
        }

        if (tokenKind == DeviceTokenKind.Teacher)
        {
            var rows = await db.TeacherDeviceTokens
                .AsNoTracking()
                .Where(t => tokens.Contains(t.FcmToken))
                .Select(t => new { t.FcmToken, t.Platform, t.TeacherId })
                .ToListAsync(cancellationToken);

            return rows
                .GroupBy(r => r.FcmToken, StringComparer.Ordinal)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var first = g.First();
                        return (first.Platform ?? string.Empty, (string?)first.TeacherId.ToString());
                    },
                    StringComparer.Ordinal);
        }

        return new Dictionary<string, (string, string?)>(StringComparer.Ordinal);
    }

    private static string AudienceLabel(DeviceTokenKind tokenKind) =>
        tokenKind switch
        {
            DeviceTokenKind.Parent => "Parent",
            DeviceTokenKind.Teacher => "Teacher",
            _ => "None",
        };

    private static string? Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.Length <= max ? value : value[..max];
    }

    private static bool ShouldPruneToken(MessagingErrorCode? errorCode) =>
        errorCode is MessagingErrorCode.Unregistered
            or MessagingErrorCode.InvalidArgument
            or MessagingErrorCode.SenderIdMismatch;

    private bool EnsureInitialized()
    {
        if (_firebaseInitialized && FirebaseApp.DefaultInstance != null)
            return true;

        lock (FirebaseInitLock)
        {
            if (_firebaseInitialized && FirebaseApp.DefaultInstance != null)
                return true;

            if (FirebaseApp.DefaultInstance != null)
            {
                _firebaseInitialized = true;
                return true;
            }

            var serviceAccountPath = ResolveServiceAccountPath();
            if (string.IsNullOrWhiteSpace(serviceAccountPath) || !File.Exists(serviceAccountPath))
            {
                logger.LogWarning(
                    "Firebase push disabled: service account file not found at {Path}",
                    serviceAccountPath);
                return false;
            }

            try
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(serviceAccountPath),
                    ProjectId = string.IsNullOrWhiteSpace(_firebaseSettings.ProjectId)
                        ? null
                        : _firebaseSettings.ProjectId,
                });
                _firebaseInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize Firebase Admin SDK");
                return false;
            }
        }
    }

    private string ResolveServiceAccountPath()
    {
        var configured = _firebaseSettings.ServiceAccountJsonPath?.Trim();
        if (string.IsNullOrEmpty(configured))
            return string.Empty;

        return Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(environment.ContentRootPath, configured);
    }
}
