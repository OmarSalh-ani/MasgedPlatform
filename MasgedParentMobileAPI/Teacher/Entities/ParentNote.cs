using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("ParentNotes")]
public class ParentNote
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    [Required]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadDate { get; set; }

    public int? ReadByTeacherId { get; set; }

    [StringLength(500)]
    public string? TeacherReply { get; set; }

    public virtual RegisterForm? RegisterForm { get; set; }
}