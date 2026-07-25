namespace MasgedTeacherMobileAPI.Dtos;

public class StudentInfoDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string FatherPhone { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public DateTime? RegistrationDate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string StudentGender { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string MaritalStatus { get; set; } = string.Empty;
    public string HealthCondition { get; set; } = string.Empty;
    public string HealthDetails { get; set; } = string.Empty;
    public string LearningDifficulties { get; set; } = string.Empty;
    public string LearningDifficultiesNotes { get; set; } = string.Empty;
}

public class AddStudentsToCircleRequestDto
{
    public List<int> StudentIds { get; set; } = [];
}
