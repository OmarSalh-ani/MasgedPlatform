using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.EventPages;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/admineventpages")]
public class AdminEventPagesController(IEventPageService eventPageService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<EventPageListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await eventPageService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("lookups")]
    public async Task<ActionResult<ApiResponseDto<List<EventPageLookupDto>>>> GetLookups(
        CancellationToken cancellationToken)
    {
        var data = await eventPageService.GetLookupsAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<EventPageLookupDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<EventPageDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await eventPageService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<EventPageDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<EventPageDto>>> Create(
        [FromForm] SaveEventPageRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await eventPageService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<EventPageDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data,
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<EventPageDto>>> Update(
        int id,
        [FromForm] SaveEventPageRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await eventPageService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<EventPageDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data,
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await eventPageService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "الصفحة غير موجودة",
            Data = deleted,
        });
    }
}
