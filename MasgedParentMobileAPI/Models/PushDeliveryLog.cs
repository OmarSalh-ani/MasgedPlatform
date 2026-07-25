using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedParentMobileAPI.Models;

/// <summary>
/// One row per FCM send attempt (or skip reason). See Scripts/CreatePushDeliveryLogs.sql.
/// </summary>
[Table("PushDeliveryLogs")]
public sealed class PushDeliveryLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    [MaxLength(40)]
    public string Source { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Context { get; set; } = string.Empty;

    [MaxLength(20)]
    public string AudienceKind { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Platform { get; set; } = string.Empty;

    /// <summary>Parent phone or teacher id.</summary>
    [MaxLength(64)]
    public string? OwnerKey { get; set; }

    [MaxLength(512)]
    public string? FcmToken { get; set; }

    public bool Success { get; set; }

    [MaxLength(100)]
    public string? ErrorCode { get; set; }

    [MaxLength(2000)]
    public string? ErrorDetail { get; set; }

    [MaxLength(200)]
    public string? MessageId { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(300)]
    public string? BodyPreview { get; set; }
}
