namespace AdminAPI.DTOs.News;

public class NewsListItemDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime NewsDate { get; set; }
}
