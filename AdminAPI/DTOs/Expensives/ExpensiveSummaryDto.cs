namespace AdminAPI.DTOs.Expensives;

public class ExpensiveSummaryDto
{
    public int TotalCount { get; set; }

    public double TotalAmount { get; set; }

    public int ThisMonthCount { get; set; }

    public double ThisMonthAmount { get; set; }

    public double AverageAmount { get; set; }
}
