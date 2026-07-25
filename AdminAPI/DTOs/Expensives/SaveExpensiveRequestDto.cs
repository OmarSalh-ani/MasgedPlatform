namespace AdminAPI.DTOs.Expensives;

public class SaveExpensiveRequestDto
{
    public string Reason { get; set; } = string.Empty;
    public double TotalAmount { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<IFormFile>? Files { get; set; }
}
