namespace AdminAPI.DTOs.PushNotifications;

public class SendAdminPushNotificationRequestDto
{
    public string Audience { get; set; } = string.Empty;

    public bool TargetAll { get; set; }

    public List<int> TeacherIds { get; set; } = [];

    public List<int> StudentIds { get; set; } = [];

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
}
