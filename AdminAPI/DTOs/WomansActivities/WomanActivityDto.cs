namespace AdminAPI.DTOs.WomansActivities;

public class WomanActivityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public bool ForGirl { get; set; }
}
