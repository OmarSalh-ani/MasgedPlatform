using AdminAPI.DTOs.About;
using AdminAPI.DTOs.Common;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminabout")]
public class AdminAboutController(IAboutService aboutService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<AboutDto?>>> Get(CancellationToken cancellationToken)
    {
        var data = await aboutService.GetAsync(cancellationToken);
        return Ok(new ApiResponseDto<AboutDto?>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponseDto<AboutDto>>> Save(
        [FromBody] UpdateAboutRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await aboutService.SaveAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<AboutDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }
}
