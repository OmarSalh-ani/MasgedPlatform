namespace AdminAPI.DTOs.WorkDays;

public class WorkDaysDto
{
    public List<int> DayNumbers { get; set; } = [];
    public List<WorkDayLabelDto> DayLabels { get; set; } = [];
}

public class WorkDayLabelDto
{
    public int Number { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}
