using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class ScanQrRequestDto : CoordinatesRequestDto
{
    [Required]
    public string QrToken { get; set; } = string.Empty;

    public bool IsDeparture { get; set; }
}

public class ScanQrResponseDto
{
    public string Message { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
}
