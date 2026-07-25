using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class RegisterTeacherFingerprintRequestDto
{
    [Required]
    [StringLength(64, MinimumLength = 64)]
    public string FingerprintHash { get; set; } = string.Empty;
}

public class ReRegisterTeacherFingerprintRequestDto
{
    [Required]
    [StringLength(64, MinimumLength = 64)]
    public string FingerprintHash { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class TeacherFingerprintStatusDto
{
    public bool HasFingerprintRegistered { get; set; }
}
