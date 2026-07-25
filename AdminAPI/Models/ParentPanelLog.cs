namespace AdminAPI.Models;

public class ParentPanelLog
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string ParentMobile { get; set; } = string.Empty;
    public DateTime AccessDateTime { get; set; }

    public virtual RegisterForm RegisterForm { get; set; } = null!;
}
