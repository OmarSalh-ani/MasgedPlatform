namespace MasgedTeacherMobileAPI.Dtos;

public class MosqueProximityDto
{
    public bool HasMosqueLocation { get; set; }
    public double DistanceMeters { get; set; }
    public string DistanceDisplay { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsWithinRadius { get; set; }
    public double MaxAllowedMeters { get; set; } = 200;
}
