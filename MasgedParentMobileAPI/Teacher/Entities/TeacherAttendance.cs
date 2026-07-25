using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("TeacherAttendance")]
public class TeacherAttendance
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public DateTime AttendanceDateTime { get; set; }

    public DateTime? DepartureDateTime { get; set; }

    public double? AttendanceLatitude { get; set; }

    public double? AttendanceLongitude { get; set; }

    public double? DepartureLatitude { get; set; }

    public double? DepartureLongitude { get; set; }
}
