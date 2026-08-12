namespace AdminAPI.DTOs.CircleVisitRating;

public class CircleVisitRatingListItemDto
{
    public int Id { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
    public string VisitTime { get; set; } = string.Empty;
    public int VisitNumberInMonth { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CircleVisitRatingItemDto
{
    public int Sequence { get; set; }
    public string Criterion { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CircleVisitRatingDetailDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int QuranCircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
    public string VisitTime { get; set; } = string.Empty;
    public int VisitNumberInMonth { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<CircleVisitRatingItemDto> Items { get; set; } = [];
}

public class CreateCircleVisitRatingItemDto
{
    public int Sequence { get; set; }
    public string Criterion { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateCircleVisitRatingRequestDto
{
    public int TeacherId { get; set; }
    public int QuranCircleId { get; set; }
    public DateTime VisitDate { get; set; }
    public string VisitTime { get; set; } = string.Empty;
    public List<CreateCircleVisitRatingItemDto> Items { get; set; } = [];
}

public class CircleVisitRatingTeacherOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CircleVisitRatingCircleOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CircleVisitRatingVisitNumberDto
{
    public int VisitNumber { get; set; }
}
