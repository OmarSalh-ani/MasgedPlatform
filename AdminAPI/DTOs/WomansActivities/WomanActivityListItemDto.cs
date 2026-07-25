namespace AdminAPI.DTOs.WomansActivities;

public class WomanActivityListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
}
