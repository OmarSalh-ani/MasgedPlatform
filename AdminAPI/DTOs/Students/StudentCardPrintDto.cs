namespace AdminAPI.DTOs.Students;

public class StudentCardPrintDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string FatherMobile { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string> CircleOptions { get; set; } = [];
}
