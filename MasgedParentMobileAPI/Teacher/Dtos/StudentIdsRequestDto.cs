using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class StudentIdsRequestDto : CoordinatesRequestDto
{
    [Required]
    [MinLength(1)]
    public List<int> StudentIds { get; set; } = [];
}
