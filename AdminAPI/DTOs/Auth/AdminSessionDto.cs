namespace AdminAPI.DTOs.Auth;

public class AdminSessionDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsGirlTeacher { get; set; }
    public bool IsViewOnly { get; set; }
    public bool IsSupervisor { get; set; }
}
