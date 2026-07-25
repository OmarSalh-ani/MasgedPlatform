using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MasgedParentMobileAPI.Configuration;
using MasgedParentMobileAPI.Models;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MasgedParentMobileAPI.Services;

public sealed class PushNotificationService
{
    private readonly FirebaseSettings _settings;
    private readonly AppDbContext _db;
    private readonly ILogger<PushNotificationService> _logger;
    private readonly IHostEnvironment _environment;
    private static readonly object FirebaseInitLock = new();
    private static bool _firebaseInitialized;

    public PushNotificationService(
        IOptions<FirebaseSettings> settings,
        AppDbContext db,
        ILogger<PushNotificationService> logger,
        IHostEnvironment environment)
    {
        _settings = settings.Value;
        _db = db;
        _logger = logger;
        _environment = environment;
    }

    public async Task SendVideoCallInviteAsync(
        IEnumerable<string> fatherPhones,
        int meetingId,
        string meetingName,
        string teacherName,
        CancellationToken cancellationToken = default)
    {
        var title = string.IsNullOrWhiteSpace(meetingName) ? "مكالمة فيديو" : meetingName.Trim();
        var body = string.IsNullOrWhiteSpace(teacherName)
            ? "المعلم يدعوك للانضمام إلى مكالمة فيديو. اضغط للانضمام."
            : $"المعلم {teacherName.Trim()} يدعوك للانضمام إلى مكالمة فيديو. اضغط للانضمام.";
        var context = $"meeting {meetingId}";

        if (!_settings.Enabled)
        {
            await PersistSkipAsync(context, DeviceTokenKind.Parent, "PushDisabled",
                "FirebaseSettings.Enabled is false", title, body, cancellationToken);
            return;
        }

        if (!EnsureInitialized())
        {
            await PersistSkipAsync(context, DeviceTokenKind.Parent, "FirebaseNotInitialized",
                "Firebase Admin SDK failed to initialize (check service account path)", title, body, cancellationToken);
            return;
        }

        var phoneVariants = fatherPhones
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .SelectMany(p => PhoneNormalizer.GetVariants(p))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (phoneVariants.Count == 0)
            return;

        var tokens = await _db.ParentDeviceTokens
            .AsNoTracking()
            .Where(t => phoneVariants.Contains(t.ParentPhone))
            .Select(t => t.FcmToken)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            _logger.LogInformation(
                "No FCM tokens for meeting {MeetingId} ({PhoneCount} phones).",
                meetingId,
                phoneVariants.Count);
            await PersistSkipAsync(context, DeviceTokenKind.Parent, "NoTokens",
                $"No device tokens for {phoneVariants.Count} phone variant(s)", title, body, cancellationToken);
            return;
        }

        var data = new Dictionary<string, string>
        {
            ["kind"] = "meet",
            ["meetingId"] = meetingId.ToString(),
            ["title"] = title,
        };

        foreach (var batch in tokens.Chunk(500))
        {
            try
            {
                var message = new MulticastMessage
                {
                    Tokens = batch.ToList(),
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body,
                    },
                    Data = data,
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            ChannelId = "masged_meetings",
                            ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                            Color = "#4A9B8F",
                            Icon = "ic_notification",
                        },
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Sound = "default",
                            ContentAvailable = true,
                        },
                    },
                };

                var response = await FirebaseMessaging.DefaultInstance
                    .SendEachForMulticastAsync(message, cancellationToken);

                await LogAndPruneMulticastFailuresAsync(
                    response,
                    batch,
                    context,
                    DeviceTokenKind.Parent,
                    title,
                    body,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FCM send failed for meeting {MeetingId}", meetingId);
                await PersistSkipAsync(context, DeviceTokenKind.Parent, "SendException",
                    Truncate(ex.Message, 2000), title, body, cancellationToken);
            }
        }
    }

    public async Task SendChatMessageToParentAsync(
        string parentPhone,
        int teacherId,
        int studentId,
        string messageText,
        CancellationToken cancellationToken = default)
    {
        const string title = "رسالة جديدة";
        var body = TruncatePreview(messageText, max: 50);
        var context = $"parent-chat t{teacherId}/s{studentId}";

        if (!_settings.Enabled)
        {
            await PersistSkipAsync(context, DeviceTokenKind.Parent, "PushDisabled",
                "FirebaseSettings.Enabled is false", title, body, cancellationToken);
            return;
        }

        if (!EnsureInitialized())
        {
            await PersistSkipAsync(context, DeviceTokenKind.Parent, "FirebaseNotInitialized",
                "Firebase Admin SDK failed to initialize (check service account path)", title, body, cancellationToken);
            return;
        }

        var phoneVariants = PhoneNormalizer.GetVariants(parentPhone)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (phoneVariants.Count == 0)
            return;

        var tokens = await _db.ParentDeviceTokens
            .AsNoTracking()
            .Where(t => phoneVariants.Contains(t.ParentPhone))
            .Select(t => t.FcmToken)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            _logger.LogInformation(
                "No FCM tokens for parent chat (teacher {TeacherId}, student {StudentId}).",
                teacherId,
                studentId);
            await PersistSkipAsync(context, DeviceTokenKind.Parent, "NoTokens",
                $"No device tokens for parent phone variants ({phoneVariants.Count})", title, body, cancellationToken);
            return;
        }

        var studentName = await _db.RegisterForms.AsNoTracking()
            .Where(s => s.Id == studentId)
            .Select(s => s.StudentName)
            .FirstOrDefaultAsync(cancellationToken);

        var teacherName = await _db.Teachers.AsNoTracking()
            .Where(t => t.Id == teacherId)
            .Select(t => t.Name)
            .FirstOrDefaultAsync(cancellationToken);

        var data = BuildChatData(
            teacherId,
            studentId,
            title,
            teacherName,
            studentName,
            parentPhone);

        await SendMulticastAsync(
            tokens,
            title,
            body,
            data,
            "masged_chat",
            DeviceTokenKind.Parent,
            cancellationToken);
    }

    public async Task SendChatMessageToTeacherAsync(
        int teacherId,
        int studentId,
        string messageText,
        CancellationToken cancellationToken = default)
    {
        var context = $"teacher-chat t{teacherId}/s{studentId}";
        var body = TruncatePreview(messageText, max: 50);

        if (!_settings.Enabled)
        {
            await PersistSkipAsync(context, DeviceTokenKind.Teacher, "PushDisabled",
                "FirebaseSettings.Enabled is false", null, body, cancellationToken);
            return;
        }

        if (!EnsureInitialized())
        {
            await PersistSkipAsync(context, DeviceTokenKind.Teacher, "FirebaseNotInitialized",
                "Firebase Admin SDK failed to initialize (check service account path)", null, body, cancellationToken);
            return;
        }

        var tokens = await _db.TeacherDeviceTokens
            .AsNoTracking()
            .Where(t => t.TeacherId == teacherId)
            .Select(t => t.FcmToken)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            _logger.LogInformation(
                "No FCM tokens for teacher chat (teacher {TeacherId}, student {StudentId}).",
                teacherId,
                studentId);
            await PersistSkipAsync(context, DeviceTokenKind.Teacher, "NoTokens",
                $"No device tokens for teacher {teacherId}", null, body, cancellationToken);
            return;
        }

        var student = await _db.RegisterForms.AsNoTracking()
            .Where(s => s.Id == studentId)
            .Select(s => new { s.StudentName, s.FatherPhone })
            .FirstOrDefaultAsync(cancellationToken);

        var studentName = student?.StudentName?.Trim();
        var parentPhone = string.IsNullOrWhiteSpace(student?.FatherPhone)
            ? string.Empty
            : PhoneNormalizer.ToCanonical(student.FatherPhone);

        var title = string.IsNullOrWhiteSpace(studentName)
            ? "ولي الأمر"
            : $"ولي أمر {studentName}";

        var data = BuildChatData(
            teacherId,
            studentId,
            title,
            teacherName: null,
            studentName,
            parentPhone);

        await SendMulticastAsync(
            tokens,
            title,
            body,
            data,
            "masged_chat",
            DeviceTokenKind.Teacher,
            cancellationToken);
    }

    private static string TruncatePreview(string text, int max = 50)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "رسالة جديدة";

        text = text.Trim();
        return text.Length <= max ? text : text[..max] + "…";
    }

    private static Dictionary<string, string> BuildChatData(
        int teacherId,
        int studentId,
        string title,
        string? teacherName,
        string? studentName,
        string? parentPhone)
    {
        var data = new Dictionary<string, string>
        {
            ["kind"] = "chat",
            ["teacherId"] = teacherId.ToString(),
            ["studentId"] = studentId.ToString(),
            ["title"] = title,
        };

        if (!string.IsNullOrWhiteSpace(teacherName))
            data["teacherName"] = teacherName.Trim();
        if (!string.IsNullOrWhiteSpace(studentName))
            data["studentName"] = studentName.Trim();
        if (!string.IsNullOrWhiteSpace(parentPhone))
            data["parentPhone"] = parentPhone.Trim();

        return data;
    }

    private async Task SendMulticastAsync(
        IReadOnlyList<string> tokens,
        string title,
        string body,
        Dictionary<string, string> data,
        string channelId,
        DeviceTokenKind tokenKind,
        CancellationToken cancellationToken)
    {
        foreach (var batch in tokens.Chunk(500))
        {
            try
            {
                var message = new MulticastMessage
                {
                    Tokens = batch.ToList(),
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body,
                    },
                    Data = data,
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            ChannelId = channelId,
                            ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                            Color = "#4A9B8F",
                            Icon = "ic_notification",
                        },
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Sound = "default",
                            ContentAvailable = true,
                        },
                    },
                };

                var response = await FirebaseMessaging.DefaultInstance
                    .SendEachForMulticastAsync(message, cancellationToken);

                await LogAndPruneMulticastFailuresAsync(
                    response,
                    batch,
                    channelId,
                    tokenKind,
                    title,
                    body,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FCM send failed for channel {Channel}", channelId);
                await PersistSkipAsync(channelId, tokenKind, "SendException",
                    Truncate(ex.Message, 2000), title, body, cancellationToken);
            }
        }
    }

    private enum DeviceTokenKind
    {
        None,
        Parent,
        Teacher,
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
        if (response.FailureCount > 0)
        {
            _logger.LogWarning(
                "FCM multicast partial failure for {Context}: {Success}/{Total}",
                context,
                response.SuccessCount,
                tokens.Count);
        }

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
                    Source = "MobileAPI",
                    Context = Truncate(context, 200)!,
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

            _logger.LogWarning(
                "FCM delivery failed ({Context}): error={ErrorCode} tokenPrefix={TokenPrefix} detail={Detail}",
                context,
                errorCodeText,
                TokenPrefix(token),
                detail);

            logs.Add(new PushDeliveryLog
            {
                CreatedAt = now,
                Source = "MobileAPI",
                Context = Truncate(context, 200)!,
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
            _db.PushDeliveryLogs.AddRange(logs);
            await _db.SaveChangesAsync(cancellationToken);
        }

        if (staleTokens.Count == 0 || tokenKind == DeviceTokenKind.None)
            return;

        if (tokenKind == DeviceTokenKind.Parent)
        {
            var rows = await _db.ParentDeviceTokens
                .Where(t => staleTokens.Contains(t.FcmToken))
                .ToListAsync(cancellationToken);

            if (rows.Count > 0)
            {
                _db.ParentDeviceTokens.RemoveRange(rows);
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Removed {Count} stale parent FCM token(s) after {Context}.",
                    rows.Count,
                    context);
            }
        }
        else
        {
            var rows = await _db.TeacherDeviceTokens
                .Where(t => staleTokens.Contains(t.FcmToken))
                .ToListAsync(cancellationToken);

            if (rows.Count > 0)
            {
                _db.TeacherDeviceTokens.RemoveRange(rows);
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Removed {Count} stale teacher FCM token(s) after {Context}.",
                    rows.Count,
                    context);
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
            _db.PushDeliveryLogs.Add(new PushDeliveryLog
            {
                CreatedAt = DateTime.UtcNow,
                Source = "MobileAPI",
                Context = Truncate(context, 200) ?? context,
                AudienceKind = AudienceLabel(tokenKind),
                Platform = string.Empty,
                Success = false,
                ErrorCode = Truncate(errorCode, 100),
                ErrorDetail = Truncate(detail, 2000),
                Title = Truncate(title, 200),
                BodyPreview = Truncate(body, 300),
            });
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist push skip log for {Context}", context);
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
            var rows = await _db.ParentDeviceTokens
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
            var rows = await _db.TeacherDeviceTokens
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

    private static string TokenPrefix(string token) =>
        token.Length <= 12 ? token : token[..12] + "…";

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
            if (string.IsNullOrWhiteSpace(serviceAccountPath)
                || !File.Exists(serviceAccountPath))
            {
                _logger.LogWarning(
                    "Firebase push disabled: service account file not found at {Path}",
                    serviceAccountPath);
                return false;
            }

            try
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(serviceAccountPath),
                    ProjectId = string.IsNullOrWhiteSpace(_settings.ProjectId)
                        ? null
                        : _settings.ProjectId,
                });
                _firebaseInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase Admin SDK");
                return false;
            }
        }
    }

    private string ResolveServiceAccountPath()
    {
        var configured = _settings.ServiceAccountJsonPath?.Trim();
        if (string.IsNullOrEmpty(configured))
            return string.Empty;

        return Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(_environment.ContentRootPath, configured);
    }
}
