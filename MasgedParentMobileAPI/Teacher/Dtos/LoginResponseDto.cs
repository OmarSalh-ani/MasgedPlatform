namespace MasgedTeacherMobileAPI.Dtos;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int Id { get; set; }
    public bool IsAdmin { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsGirlTeacher { get; set; }
    public int CircleId { get; set; }
    public string Name { get; set; } = string.Empty;
}
