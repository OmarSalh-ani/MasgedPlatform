using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class LocationRequestDto
{
    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    /// <summary>SHA-256 hex hash of the teacher-bound enrollment secret (see TeacherFingerprintHashHelper).</summary>
    [Required]
    [StringLength(64, MinimumLength = 64)]
    public string FingerprintHash { get; set; } = string.Empty;
}
