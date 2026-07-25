using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("TeachersAdminNotes")]
public class TeachersAdminNote
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    [Required]
    public string Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadTime { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
