namespace AdminAPI.DTOs.EventPageResponses;

public class EventPageResponseFiltersDto
{
    public string? ActivityName { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
