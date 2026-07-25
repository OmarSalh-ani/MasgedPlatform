using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Subscribe;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/adminsubscribe")]
public class AdminSubscribeController(ISubscribeService subscribeService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<SubmitSubscribeResponseDto>>> Submit(
        [FromBody] SubmitSubscribeRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await subscribeService.SubmitAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<SubmitSubscribeResponseDto>
        {
            Success = true,
            Message = "تم التسجيل بنجاح",
            Data = data,
        });
    }
}
