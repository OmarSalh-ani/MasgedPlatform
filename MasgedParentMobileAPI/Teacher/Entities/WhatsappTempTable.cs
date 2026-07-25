using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("whatsapp_temp_table")]
public class WhatsappTempTable
{
    public int id { get; set; }

    public string? message { get; set; }

    [StringLength(50)]
    public string? mobile { get; set; }

    public int? IsGirl { get; set; }
}
