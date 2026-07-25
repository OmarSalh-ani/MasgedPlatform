namespace AdminAPI.DTOs.Home;

public class CreateHomeCircleRequestDto
{
    public string CircleName { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public List<int> StudentIds { get; set; } = [];
}
