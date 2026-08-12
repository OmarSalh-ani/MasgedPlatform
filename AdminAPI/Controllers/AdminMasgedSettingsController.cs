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
