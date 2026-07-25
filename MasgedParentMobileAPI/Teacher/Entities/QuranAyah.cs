using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("QuranAyah")]
public class QuranAyah
{
    public int Id { get; set; }

    public int SurahId { get; set; }

    public int AyahNumber { get; set; }
}
