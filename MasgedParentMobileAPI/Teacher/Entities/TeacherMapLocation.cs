using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("TeacherMapLocation")]
public class TeacherMapLocation
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    [StringLength(50)]
    public string? Latitude { get; set; }

    [StringLength(50)]
    public string? Longitude { get; set; }
}
