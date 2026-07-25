namespace AdminAPI.DTOs.WomansActivities;

public class SaveWomanActivityRequestDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
}
