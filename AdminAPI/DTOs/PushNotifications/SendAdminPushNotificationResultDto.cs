namespace AdminAPI.DTOs.PushNotifications;

public class SendAdminPushNotificationResultDto
{
    public int RecipientsResolved { get; set; }

    public int RecipientsWithoutTokens { get; set; }

    public int TokensAttempted { get; set; }

    public int SuccessCount { get; set; }

    public int FailureCount { get; set; }
}
