using System.ComponentModel.DataAnnotations;

namespace MasgedTeacherMobileAPI.Dtos;

public class SaveAttendanceRequestDto : StudentIdsRequestDto
{
    public string? AttendanceDate { get; set; }
}
