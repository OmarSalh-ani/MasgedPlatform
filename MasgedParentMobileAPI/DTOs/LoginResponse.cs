namespace MasgedParentMobileAPI.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int ParentId { get; set; }
    public string FatherName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
