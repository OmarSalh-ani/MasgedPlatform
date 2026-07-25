namespace AdminAPI.DTOs.Home;



public class HomeStudentNameLookupFiltersDto

{

    public string? Search { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

}


