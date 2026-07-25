namespace AdminAPI.DTOs.Mosques;

public class SaveMosqueRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? GoogleMapsUrl { get; set; }

    public int SortOrder { get; set; }

    public IFormFile? Image { get; set; }
}
