using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("CircleDay")]
public class CircleDay
{
    public int Id { get; set; }

    public int CircleId { get; set; }

    public int DayNumber { get; set; }
}
