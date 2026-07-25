namespace MasgedTeacherMobileAPI.Dtos;

public class SaveStudentNoteRequestDto
{
    public string NotesText { get; set; } = string.Empty;
    public bool IsWarning { get; set; }
}
