namespace AdminAPI.DTOs.Expensives;

public class ExpensiveDto
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public double TotalAmount { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ExpensiveAttachmentDto> Attachments { get; set; } = [];
}
