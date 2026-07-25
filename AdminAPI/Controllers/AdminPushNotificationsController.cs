using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.DTOs.PushNotifications;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminpushnotifications")]
public class AdminPushNotificationsController(
    IAdminPushNotificationService service,
    IValidator<SendAdminPushNotificationRequestDto> sendValidator) : ControllerBase
{
    [HttpGet("teachers")]
    public async Task<ActionResult<ApiResponseDto<List<PushNotificationTeacherOptionDto>>>> GetTeachers(
        CancellationToken cancellationToken)
    {
        var data = await service.GetTeachersAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<PushNotificationTeacherOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("students")]
    public async Task<ActionResult<PagedResultDto<HomeStudentListItemDto>>> GetStudents(
        [FromQuery] HomeListFiltersDto filters,
        CancellationToken cancellationToken) =>
        Ok(await service.GetStudentsAsync(filters, cancellationToken));

    [HttpGet("students/filter-options")]
    public async Task<ActionResult<ApiResponseDto<HomeFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken)
    {
        var data = await service.GetFilterOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<HomeFilterOptionsDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("send")]
    public async Task<ActionResult<ApiResponseDto<SendAdminPushNotificationResultDto>>> Send(
        [FromBody] SendAdminPushNotificationRequestDto request,
        CancellationToken cancellationToken)
    {
        await sendValidator.ValidateAndThrowAsync(request, cancellationToken);
        var data = await service.SendAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<SendAdminPushNotificationResultDto>
        {
            Success = true,
            Message = "تم إرسال الإشعار",
            Data = data,
        });
    }
}
