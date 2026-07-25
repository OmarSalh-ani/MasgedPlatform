namespace AdminAPI.DTOs.SpecialStudentsReport;

public class SpecialStudentsReportItemDto
{
    public string StudentName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int? CircleId { get; set; }
}
