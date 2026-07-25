namespace MasgedTeacherMobileAPI.Dtos;

public class RevisePageDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public List<IdNameDto> Surahs { get; set; } = [];
    public List<StudentReviewDto> Reviews { get; set; } = [];
}

public class StudentReviewDto
{
    public int Id { get; set; }
    public int CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public bool CanEdit { get; set; }
    public string ReviewType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string TestFrom { get; set; } = string.Empty;
    public string TestTo { get; set; } = string.Empty;
    public string? SurahName { get; set; }
    public string IsDone { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ParentNotes { get; set; }
    public string? IsSaveDone { get; set; }
    public int? SurahId { get; set; }
}

public class ReviseMemorizationInputDto
{
    public int SurahId { get; set; }
    public string TestFrom { get; set; } = string.Empty;
    public string TestTo { get; set; } = string.Empty;
    public string? NextMemorization { get; set; }
    public bool IsSaveCompleted { get; set; }
}

public class ReviseRevisionInputDto
{
    public string TestFrom { get; set; } = string.Empty;
    public string TestTo { get; set; } = string.Empty;
    public string? NextRevise { get; set; }
    public bool IsRevisionCompleted { get; set; }
}

public class CreateReviseRequestDto
{
    public string? ParentNotes { get; set; }
    public ReviseMemorizationInputDto? Memorization { get; set; }
    public ReviseRevisionInputDto? Revision { get; set; }
}

public class UpdateReviseRequestDto
{
    public string? ParentNotes { get; set; }
    public ReviseMemorizationInputDto? Memorization { get; set; }
    public ReviseRevisionInputDto? Revision { get; set; }
}
