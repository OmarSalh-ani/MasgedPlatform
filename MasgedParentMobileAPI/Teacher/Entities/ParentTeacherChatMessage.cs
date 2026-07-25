using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("ParentTeacherChatMessages")]
public class ParentTeacherChatMessage
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string ParentPhone { get; set; } = string.Empty;

    public int TeacherId { get; set; }

    public byte SenderType { get; set; }

    [Required]
    [StringLength(2000)]
    public string MessageText { get; set; } = string.Empty;

    public int? StudentId { get; set; }

    public DateTime SentAt { get; set; }

    public bool IsReadByParent { get; set; }

    public bool IsReadByTeacher { get; set; }

    public virtual Teacher? Teacher { get; set; }

    public virtual RegisterForm? RegisterForm { get; set; }
}
