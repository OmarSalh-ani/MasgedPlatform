namespace MasgedParentMobileAPI.DTOs;

public sealed class RegisterDeviceTokenRequestDto
{
    public string FcmToken { get; set; } = string.Empty;

    public string Platform { get; set; } = string.Empty;
}
