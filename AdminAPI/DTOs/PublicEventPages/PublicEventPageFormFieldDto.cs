namespace AdminAPI.DTOs.PublicEventPages;

public class PublicEventPageFormFieldDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public List<string> Options { get; set; } = [];
}
