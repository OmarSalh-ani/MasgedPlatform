namespace AdminAPI.Models;



public class QuranSurah

{

    public int Id { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public int? SortOrder { get; set; }



    public virtual ICollection<QuranAyah> QuranAyahs { get; set; } = [];

    public virtual ICollection<StudentPlanMemorizing> StudentPlanMemorizings { get; set; } = [];

    public virtual ICollection<StudentPlanRevise> StudentPlanRevises { get; set; } = [];

}

