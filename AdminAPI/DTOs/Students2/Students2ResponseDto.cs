namespace AdminAPI.DTOs.Students2;

public class Students2ResponseDto
{
    public List<Students2ListItemDto> Items { get; set; } = [];
    public Students2StatsDto Stats { get; set; } = new();
}
