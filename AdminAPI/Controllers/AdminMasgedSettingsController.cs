using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.MasgedSettings;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminmasgedsettings")]
public class AdminMasgedSettingsController(IMasgedSettingsService masgedSettingsService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<MasgedSettingsDto?>>> Get(CancellationToken cancellationToken)
    {
        var data = await masgedSettingsService.GetAsync(cancellationToken);
        return Ok(new ApiResponseDto<MasgedSettingsDto?>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [AllowAnonymous]
    [HttpGet("setup-status")]
    public async Task<ActionResult<ApiResponseDto<SetupStatusDto>>> GetSetupStatus(
        CancellationToken cancellationToken)
    {
        var data = await masgedSettingsService.GetSetupStatusAsync(cancellationToken);
        return Ok(new ApiResponseDto<SetupStatusDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [AllowAnonymous]
    [HttpPost("setup")]
    public async Task<ActionResult<ApiResponseDto<MasgedSettingsDto>>> CompleteSetup(
        [FromForm] FirstTimeSetupRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await masgedSettingsService.CompleteSetupAsync(request, cancellationToken);
            return Ok(new ApiResponseDto<MasgedSettingsDto>
            {
                Success = true,
                Message = "تم إكمال الإعداد",
                Data = data,
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<MasgedSettingsDto>
            {
                Success = false,
                Message = ex.Message,
                Errors = [ex.Message],
            });
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ApiResponseDto<MasgedSettingsDto>>> Save(
        [FromForm] UpdateMasgedSettingsRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await masgedSettingsService.SaveAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<MasgedSettingsDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data,
        });
    }
}
