using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class ChangePasswordRequestDto
{
    [Required]
    [MinLength(1)]
    public string NewPassword { get; set; } = string.Empty;
}
