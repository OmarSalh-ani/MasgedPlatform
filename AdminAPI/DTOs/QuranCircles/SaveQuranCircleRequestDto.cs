namespace AdminAPI.DTOs.QuranCircles;

public class SaveQuranCircleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public bool ForGirls { get; set; }
}
