namespace AdminAPI.DTOs.Expensives;

public class ExpensiveListItemDto
{
    public int Id { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string Supplier { get; set; } = string.Empty;

    public double TotalAmount { get; set; }

    public bool ForGirls { get; set; }
}
