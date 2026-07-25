using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class CoordinatesRequestDto
{
    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
}
