namespace AdminAPI.DTOs.Home;

public class HomeListFiltersDto
{
    public string? StudentName { get; set; }
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public int? CircleId { get; set; }
    public string? FatherMobile { get; set; }
    public int? WomanActivityTypeId { get; set; }
    public string? FormStatus { get; set; }
    public bool SpecialOnly { get; set; }
    public bool EliteOnly { get; set; }
    public bool BoysOnly { get; set; }
    public bool GirlsOnly { get; set; }
    public int? CircleQuery { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
