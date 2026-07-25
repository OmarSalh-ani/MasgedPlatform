namespace MasgedTeacherMobileAPI.Dtos;

public class QuranLineDto
{
    public int LineNumber { get; set; }
    public string Text { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
}
