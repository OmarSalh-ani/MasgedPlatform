namespace AdminAPI.Models;

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UsersManage { get; set; }
    public bool? IsGirlTeacher { get; set; }
    public bool IsViewOnly { get; set; }
    public string? Mobile { get; set; }
    public string? Image { get; set; }
    public decimal? BaseSalary { get; set; }
}
