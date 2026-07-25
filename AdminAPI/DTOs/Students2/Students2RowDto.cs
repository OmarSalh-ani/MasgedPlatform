namespace AdminAPI.DTOs.Students2;

public class Students2RowDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public int Age { get; set; }
    public string StudentGender { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string? CircleName { get; set; }
    public bool MrkzStudent { get; set; }=false;
    public DateTime? CreatedAt { get; set; }
    public string? PhotoPath { get; set; }
}
