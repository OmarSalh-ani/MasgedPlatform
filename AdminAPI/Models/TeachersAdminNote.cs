namespace AdminAPI.Models;

public class TeachersAdminNote
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public string Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadTime { get; set; }

    public Teacher? Teacher { get; set; }
}
