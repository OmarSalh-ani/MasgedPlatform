namespace AdminAPI.DTOs.TeacherSendNotes;

public class CreateTeacherSendNotesRequestDto
{
    public List<int> TeacherIds { get; set; } = [];
    public string Note { get; set; } = string.Empty;
}
