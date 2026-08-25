namespace AdminAPI.DTOs.PublicEventPages;

public class SubmitEventPageAnswerDto
{
    public int FieldId { get; set; }
    public string? Value { get; set; }
    public List<string>? Values { get; set; }
}
