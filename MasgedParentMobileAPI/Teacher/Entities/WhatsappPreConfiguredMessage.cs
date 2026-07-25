using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("WhatsappPreConfiguredMessages")]
public class WhatsappPreConfiguredMessage
{
    public int Id { get; set; }

    [Required]
    [StringLength(2000)]
    public string WhatsappMessage { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Event { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
