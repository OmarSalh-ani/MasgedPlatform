namespace AdminAPI.DTOs.EventPageResponses;

public class EventPageResponseListItemDto
{
    public int Id { get; set; }
    public int EventPageId { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public List<EventPageResponseValueDto> Values { get; set; } = [];
}
