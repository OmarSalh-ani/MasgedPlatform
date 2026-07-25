namespace AdminAPI.DTOs.TeacherSalaries;

public class TeacherSalaryFilterOptionsDto
{
    public List<TeacherSalaryOptionDto> Months { get; set; } = [];
    public List<TeacherSalaryOptionDto> Years { get; set; } = [];
    public List<TeacherSalaryOptionDto> Teachers { get; set; } = [];
    public int DefaultMonth { get; set; }
    public int DefaultYear { get; set; }
}

public class TeacherSalaryOptionDto
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class TeacherSalaryFormTeacherDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? BaseSalary { get; set; }
}
