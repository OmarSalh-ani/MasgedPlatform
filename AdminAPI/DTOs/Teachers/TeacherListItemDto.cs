namespace AdminAPI.DTOs.Teachers;

public class TeacherListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CircleCount { get; set; }
    public string? Mobile { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UsersManage { get; set; }
    public string? ImageUrl { get; set; }
}
