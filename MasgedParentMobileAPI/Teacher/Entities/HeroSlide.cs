using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("HeroSlide")]
public class HeroSlide
{
    public int Id { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
