namespace AdminAPI.Models;

public class EventPageResponse
{
    public int Id { get; set; }
    public int EventPageId { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }

    public EventPage? EventPage { get; set; }
    public ICollection<EventPageResponseValue> Values { get; set; } = new List<EventPageResponseValue>();
}
