namespace AdminAPI.DTOs.Students2;

public class Students2ListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string RegistrationType { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
