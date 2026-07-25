namespace MasgedTeacherMobileAPI.Dtos;

public class FilteredStudentsResponseDto
{
    public List<StudentDto> Students { get; set; } = [];
    public int Count { get; set; }
    public bool ShowOnlySpecial { get; set; }
    public bool ShowOnlyElite { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
}

public class SaveBulkNotesRequestDto
{
    public List<int> StudentIds { get; set; } = [];
    public string NoteText { get; set; } = string.Empty;
    public bool IsWarning { get; set; }
}

public class SaveBulkNotesResponseDto
{
    public int SentCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class TeacherAdminNoteDto
{
    public int Id { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedAtFormatted { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadTime { get; set; }
    public string? ReadTimeFormatted { get; set; }
}
