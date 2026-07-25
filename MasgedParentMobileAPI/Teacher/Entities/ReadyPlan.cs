using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("ReadyPlan")]
public class ReadyPlan
{
    public int Id { get; set; }

    public int PlanLevelId { get; set; }

    public int FromSurahId { get; set; }

    public int ToSurahId { get; set; }

    public int? FromAyah { get; set; }

    public int? ToAyah { get; set; }

    public int? FromJozz { get; set; }

    public int? ToJozz { get; set; }

    [Column(TypeName = "date")]
    public DateTime FromDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime ToDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedByTeacherId { get; set; }

    public virtual PlanLevel? PlanLevel { get; set; }
}
