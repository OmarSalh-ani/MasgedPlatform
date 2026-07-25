using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.SocialLink;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminsociallink")]
public class AdminSocialLinkController(ISocialLinkService socialLinkService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<SocialLinkDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await socialLinkService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<SocialLinkDto>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpGet("next-sort-order")]
    public async Task<ActionResult<ApiResponseDto<int>>> GetNextSortOrder(
        CancellationToken cancellationToken)
    {
        var data = await socialLinkService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponseDto<int>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<SocialLinkDto>>> Create(
        [FromBody] SaveSocialLinkRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await socialLinkService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<SocialLinkDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<SocialLinkDto>>> Update(
        int id,
        [FromBody] SaveSocialLinkRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await socialLinkService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<SocialLinkDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await socialLinkService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "الرابط غير موجود",
            Data = deleted
        });
    }
}
