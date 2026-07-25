namespace MasgedParentMobileAPI.DTOs;

public class StudentRegistrationRequestDto
{
    public string Mode { get; set; } = "default";
    public string ParentPhoneCountryIso { get; set; } = string.Empty;
    public string ParentPhone1 { get; set; } = string.Empty;
    public string? ParentPhone2 { get; set; }
    public string? ParentPhone2CountryIso { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public List<StudentRegistrationEntryDto> Students { get; set; } = [];
}
