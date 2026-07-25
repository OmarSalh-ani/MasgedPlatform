namespace MasgedParentMobileAPI.DTOs;

public sealed class ParentNotificationDto
{
    public string Kind { get; set; } = string.Empty;

    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    /// <summary>For meet notifications: false when teacher ended the call.</summary>
    public bool CanJoin { get; set; } = true;

    public DateTime? EndedAt { get; set; }
}
