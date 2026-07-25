namespace MasgedTeacherMobileAPI.Dtos;

public class StudentTestsPageDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public List<StudentTestListItemDto> Tests { get; set; } = [];
}

public class StudentTestListItemDto
{
    public int TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string SurahName { get; set; } = string.Empty;
    public string HezbNumber { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string TestDegree { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public bool CanEdit { get; set; }
}

public class StudentTestDetailDto
{
    public int TestId { get; set; }
    public int StudentId { get; set; }
    public int CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public bool CanEdit { get; set; }
    public string TestDate { get; set; } = string.Empty;
    public string FinalResult { get; set; } = string.Empty;
    public string SurahName { get; set; } = string.Empty;
    public string HezbNumber { get; set; } = string.Empty;
    public string FromSurah { get; set; } = string.Empty;
    public string ToSurah { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal? MemorizationScore { get; set; }
    public decimal? TajweedScore { get; set; }
    public decimal? RevisionScore { get; set; }
    public decimal? TotalScore { get; set; }
    public string? Grade { get; set; }
    public List<StudentTestQuestionDto> Questions { get; set; } = [];
}

public class StudentTestQuestionDto
{
    public string? QuestionName { get; set; }
    public int TestDegree { get; set; }
    public int QuestionOrder { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class SaveStudentTestRequest
{
    public DateTime? TestDate { get; set; }
    public string? SurahName { get; set; }
    public string? HezbNumber { get; set; }
    public string? Notes { get; set; }
    public int? MemorizationScore { get; set; }
    public int? TajweedScore { get; set; }
    public int? RevisionScore { get; set; }
    public int? TotalScore { get; set; }
    public string? Grade { get; set; }
    public List<StudentTestRowDto>? TestRows { get; set; }
}

public class StudentTestRowDto
{
    public string? SurahName { get; set; }
    public string? HezbNumber { get; set; }
    public string? FromSurah { get; set; }
    public string? ToSurah { get; set; }
    public string? Question { get; set; }
    public string? Degree { get; set; }
    public int RowNumber { get; set; }
}
