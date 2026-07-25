using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Integrations;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminintegrations")]
public class AdminIntegrationsController(IIntegrationSettingsService integrationSettingsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IntegrationSettingsDto>>> Get(
        CancellationToken cancellationToken)
    {
        var data = await integrationSettingsService.GetAsync(cancellationToken);
        return Ok(new ApiResponseDto<IntegrationSettingsDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponseDto<IntegrationSettingsDto>>> Save(
        [FromBody] UpdateIntegrationSettingsRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await integrationSettingsService.SaveAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<IntegrationSettingsDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data,
        });
    }
}
