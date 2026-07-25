namespace AdminAPI.Models;



public class QuranAyah

{

    public int Id { get; set; }

    public int SurahId { get; set; }

    public int AyahNumber { get; set; }



    public virtual QuranSurah Surah { get; set; } = null!;

}

