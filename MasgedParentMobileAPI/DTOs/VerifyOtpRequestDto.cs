namespace MasgedParentMobileAPI.DTOs;

public class VerifyOtpRequestDto
{
    public string FatherPhone { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;
}
