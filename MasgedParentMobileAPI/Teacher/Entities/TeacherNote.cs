using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("TeacherNotes")]
public class TeacherNote
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int TeacherId { get; set; }

    [Required]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadDate { get; set; }

    public int? ReadByParentId { get; set; }

    public bool IsWarning { get; set; }
}