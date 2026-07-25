namespace AdminAPI.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsGirlTeacher { get; set; }
    public bool IsViewOnly { get; set; }
    public string RedirectPath { get; set; } = string.Empty;
}
