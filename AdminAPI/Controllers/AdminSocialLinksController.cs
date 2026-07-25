using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.SocialLink;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminsociallinks")]
public class AdminSocialLinksController(ISocialLinkService socialLinkService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<SocialLinkListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await socialLinkService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
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
