namespace MasgedTeacherMobileAPI.Dtos;

public class StudentCircleHistoryItemDto
{
    public int CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class FormerStudentDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime? LeftDate { get; set; }
}
