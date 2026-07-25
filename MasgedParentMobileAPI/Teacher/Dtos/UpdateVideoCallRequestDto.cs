using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class UpdateVideoCallRequestDto
{
    [Required]
    public string MeetingName { get; set; } = string.Empty;

    public DateTime? StartDateTime { get; set; }
}
