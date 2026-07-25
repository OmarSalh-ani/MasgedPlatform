using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.News;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminnewsitems")]
public class AdminNewsItemsController(INewsService newsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<NewsListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await newsService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await newsService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "الخبر غير موجود",
            Data = deleted
        });
    }
}
