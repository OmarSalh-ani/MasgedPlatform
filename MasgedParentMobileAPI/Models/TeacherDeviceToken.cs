namespace MasgedParentMobileAPI.Models;

public sealed class TeacherDeviceToken
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public string FcmToken { get; set; } = string.Empty;

    public string Platform { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}
