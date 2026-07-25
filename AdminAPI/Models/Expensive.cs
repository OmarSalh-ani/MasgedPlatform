namespace AdminAPI.Models;

public class Expensive
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public double TotalAmount { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? AttachmentsFolder { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? TeacherId { get; set; }
    public bool? ForGirls { get; set; }

    public Teacher? Teacher { get; set; }
}
