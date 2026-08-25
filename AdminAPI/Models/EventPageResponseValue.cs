namespace AdminAPI.Models;

public class EventPageResponseValue
{
    public int Id { get; set; }
    public int ResponseId { get; set; }
    public int? FieldId { get; set; }
    public string FieldLabel { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public EventPageResponse? Response { get; set; }
}
