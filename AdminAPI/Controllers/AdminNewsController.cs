using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.News;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminnews")]
public class AdminNewsController(INewsService newsService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<NewsDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await newsService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<NewsDto>
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
        var data = await newsService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponseDto<int>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<NewsDto>>> Create(
        [FromForm] SaveNewsRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await newsService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<NewsDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<NewsDto>>> Update(
        int id,
        [FromForm] SaveNewsRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await newsService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<NewsDto>
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
        var deleted = await newsService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "الخبر غير موجود",
            Data = deleted
        });
    }
}
