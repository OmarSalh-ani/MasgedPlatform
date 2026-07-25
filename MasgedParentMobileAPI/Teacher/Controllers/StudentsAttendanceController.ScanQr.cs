using System.Reflection;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

public partial class StudentsAttendanceController
{
    [HttpPost("scan-qr")]
    public async Task<IActionResult> ScanQr(
        [FromBody] ScanQrRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!await MosqueLocationHelper.IsWithinMosqueRadiusAsync(
                db, teacherId, request.Latitude, request.Longitude, cancellationToken))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                MosqueLocationHelper.OutsideMosqueMessageForStudentAttendance(
                    isDeparture: request.IsDeparture)));
        }

        if (string.IsNullOrWhiteSpace(request.QrToken)
            || !qrTokenService.TryDecryptStudentId(request.QrToken.Trim(), out var studentId))
        {
            return this.ToActionResult(GlobalResponse.BadRequest("رمز QR غير صالح"));
        }

        var student = await db.RegisterForms.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);

        if (student is null || student.QuranCircleId != circleId)
            return this.ToActionResult(GlobalResponse.BadRequest("الطالب غير موجود في حلقتك"));

        var studentName = !string.IsNullOrWhiteSpace(student.FullName)
            ? student.FullName
            : student.StudentName;

        var markResult = request.IsDeparture
            ? await SaveDepartureForMultipleStudents(new StudentIdsRequestDto
            {
                StudentIds = [studentId],
                Latitude = request.Latitude,
                Longitude = request.Longitude,
            }, cancellationToken)
            : await SaveAttendanceForMultipleStudents(new SaveAttendanceRequestDto
            {
                StudentIds = [studentId],
                Latitude = request.Latitude,
                Longitude = request.Longitude,
            }, cancellationToken);

        return WrapScanQrResponse(markResult, studentId, studentName);
    }

    private IActionResult WrapScanQrResponse(IActionResult markResult, int studentId, string studentName)
    {
        if (markResult is not ObjectResult { Value: GlobalResponse response })
            return markResult;

        if (!response.Success)
            return markResult;

        return this.ToActionResult(GlobalResponse.Ok(new ScanQrResponseDto
        {
            Message = ExtractMarkMessage(response),
            StudentId = studentId,
            StudentName = studentName,
        }));
    }

    private static string ExtractMarkMessage(GlobalResponse response)
    {
        if (response.Data is not null)
        {
            var messageProperty = response.Data.GetType().GetProperty(
                "message",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (messageProperty?.GetValue(response.Data) is string nestedMessage
                && !string.IsNullOrWhiteSpace(nestedMessage))
            {
                return nestedMessage;
            }
        }

        return string.IsNullOrWhiteSpace(response.Message)
            ? GlobalResponse.DefaultMessages.Ok
            : response.Message;
    }
}
