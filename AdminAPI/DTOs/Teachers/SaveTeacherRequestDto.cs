namespace AdminAPI.DTOs.Teachers;

public class SaveTeacherRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public decimal? BaseSalary { get; set; }
    public int? CircleId { get; set; }
    public bool IsGirlTeacher { get; set; }
    public bool UsersManage { get; set; }
    public bool IsViewOnly { get; set; }
    public bool RemoveImage { get; set; }
    public IFormFile? Image { get; set; }
    public string? SelectedMosqueIds { get; set; }
    public string? ManualLocationsJson { get; set; }
}
