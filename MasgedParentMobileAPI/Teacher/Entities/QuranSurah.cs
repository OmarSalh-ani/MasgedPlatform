using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("QuranSurah")]
public class QuranSurah
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string NameAr { get; set; } = string.Empty;

    public int? SortOrder { get; set; }
}
