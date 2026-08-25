namespace AdminAPI.Models;

public class EventPageFormField
{
    public int Id { get; set; }
    public int EventPageId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = EventPageFieldTypes.Text;
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public string? OptionsJson { get; set; }

    public EventPage? EventPage { get; set; }
}
