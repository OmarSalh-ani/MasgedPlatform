namespace AdminAPI.DTOs.Home;

public class HomeFilterOptionsDto
{
    public List<HomeLookupDto> Circles { get; set; } = [];
    public List<HomeLookupDto> TransferCircles { get; set; } = [];
    public List<HomeLookupDto> Teachers { get; set; } = [];
    public List<HomeLookupDto> WomanActivityTypes { get; set; } = [];
}
