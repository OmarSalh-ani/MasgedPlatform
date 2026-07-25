namespace MasgedTeacherMobileAPI.Dtos;

public class QuranPageResponseDto
{
    public int CurrentPage { get; set; }
    public int MaxPage { get; set; }
    public int Jozz { get; set; }
    public string PageMeta { get; set; } = string.Empty;
    public string SurahName { get; set; } = string.Empty;
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    public int? PreviousPage { get; set; }
    public int? NextPage { get; set; }
    public bool IsFiltered { get; set; }
    public int? FilterSurahId { get; set; }
    public int? FilterFromAyah { get; set; }
    public int? FilterToAyah { get; set; }
    public List<int> HighlightAyahNumbers { get; set; } = [];
    public List<QuranLineDto> Lines { get; set; } = [];
}
