using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("QuranCircle")]
public class QuranCircle
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    public int? TeacherId { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
