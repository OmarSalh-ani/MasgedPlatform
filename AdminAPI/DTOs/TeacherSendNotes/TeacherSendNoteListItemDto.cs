namespace AdminAPI.DTOs.TeacherSendNotes;

public class TeacherSendNoteListItemDto
{
    public int Id { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadTime { get; set; }
}
