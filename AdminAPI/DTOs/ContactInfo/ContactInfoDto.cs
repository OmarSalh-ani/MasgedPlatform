namespace AdminAPI.DTOs.ContactInfo;

public class ContactInfoDto
{
    public int Id { get; set; }

    public string ContactType { get; set; } = string.Empty;

    public string? Label { get; set; }

    public string Value { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
