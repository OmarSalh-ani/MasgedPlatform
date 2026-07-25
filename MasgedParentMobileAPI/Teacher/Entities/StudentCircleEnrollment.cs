using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("StudentCircleEnrollment")]
public class StudentCircleEnrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CircleId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? AssignedByTeacherId { get; set; }

    public virtual RegisterForm? RegisterForm { get; set; }

    public virtual QuranCircle? QuranCircle { get; set; }

    public virtual Teacher? AssignedByTeacher { get; set; }
}
