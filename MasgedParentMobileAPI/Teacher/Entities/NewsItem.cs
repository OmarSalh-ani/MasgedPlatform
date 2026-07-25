using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("NewsItem")]
public class NewsItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime NewsDate { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? LinkUrl { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
