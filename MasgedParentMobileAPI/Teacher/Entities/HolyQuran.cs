using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("HolyQuran")]
public class HolyQuran
{
    public int id { get; set; }

    public int jozz { get; set; }

    public int sura_no { get; set; }

    [StringLength(100)]
    public string? sura_name_ar { get; set; }

    public int page { get; set; }

    public int? line_start { get; set; }

    public int? line_end { get; set; }

    public int aya_no { get; set; }

    public string? aya_text_emlaey { get; set; }
}
