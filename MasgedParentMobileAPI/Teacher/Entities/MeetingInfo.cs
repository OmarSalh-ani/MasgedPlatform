using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MasgedTeacherMobileAPI.Enums;

namespace MasgedTeacherMobileAPI.Entities;

[Table("MeetingsInfo")]
public class MeetingInfo
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    [Required]
    public string MeetingUrl { get; set; } = string.Empty;

    public string? ApiResponse { get; set; }

    public string? MeetingName { get; set; }

    public string? TeacherName { get; set; }

    public DateTime StartDateTime { get; set; }

    public string? StudentIds { get; set; }

    public DateTime CreatedAt { get; set; }

    public MeetingStatus Status { get; set; } = MeetingStatus.Active;

    public DateTime? EndedAt { get; set; }

    public string? TeacherNotes { get; set; }
}
