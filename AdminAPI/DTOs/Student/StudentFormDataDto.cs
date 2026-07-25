namespace AdminAPI.DTOs.Student;



public class StudentFormDataDto

{

    public List<StudentLookupOptionDto> Circles { get; set; } = [];

    public List<StudentLookupOptionDto> PlanLevels { get; set; } = [];

    public bool CanModify { get; set; }

    public string DefaultRegistrationDate { get; set; } = string.Empty;

}

