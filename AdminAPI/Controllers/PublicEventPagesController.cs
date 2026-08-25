using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.PublicEventPages;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/publiceventpages")]
public class PublicEventPagesController(IPublicEventPageService publicEventPageService) : ControllerBase
{
    [HttpGet("{slug}")]
    public async Task<ActionResult<ApiResponseDto<PublicEventPageDto>>> GetBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        var data = await publicEventPageService.GetBySlugAsync(slug, cancellationToken);
        return Ok(new ApiResponseDto<PublicEventPageDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("{slug}/register")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Register(
        string slug,
        [FromBody] SubmitEventPageRegistrationRequestDto request,
        CancellationToken cancellationToken)
    {
        await publicEventPageService.SubmitRegistrationAsync(slug, request, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = true,
            Message = "تم التسجيل بنجاح",
            Data = true,
        });
    }
}
