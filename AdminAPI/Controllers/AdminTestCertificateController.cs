using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.PushNotifications;
using AdminAPI.DTOs.TestCertificate;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/admintestcertificate")]
public class AdminTestCertificateController(
    ITestCertificateService testCertificateService,
    IValidator<SendTestCertificateNotificationRequestDto> notifyValidator) : ControllerBase
{
    [HttpGet("{testId:int}")]
    public async Task<ActionResult<ApiResponseDto<TestCertificateDto>>> GetByTestId(
        int testId,
        CancellationToken cancellationToken)
    {
        var data = await testCertificateService.GetByTestIdAsync(testId, cancellationToken);
        return Ok(new ApiResponseDto<TestCertificateDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("{testId:int}/notify")]
    public async Task<ActionResult<ApiResponseDto<SendAdminPushNotificationResultDto>>> Notify(
        int testId,
        [FromBody] SendTestCertificateNotificationRequestDto request,
        CancellationToken cancellationToken)
    {
        await notifyValidator.ValidateAndThrowAsync(request, cancellationToken);
        var data = await testCertificateService.SendNotificationAsync(testId, request, cancellationToken);
        return Ok(new ApiResponseDto<SendAdminPushNotificationResultDto>
        {
            Success = true,
            Message = "تم إرسال الإشعار",
            Data = data,
        });
    }
}
