#nullable disable
namespace MasgedParentMobileAPI.Models;

/// <summary>
/// Holds a short-lived registration OTP challenge for enrolling a parent whose children
/// exist in RegisterForm but passwords are not set yet (first-time activation).
/// </summary>
public class ParentRegistrationOtp
{
    public string CanonicalPhone { get; set; }

    public string FatherName { get; set; }

    public string PasswordPlain { get; set; }

    public string OtpCode { get; set; }

    public DateTime ExpiresUtc { get; set; }
}
