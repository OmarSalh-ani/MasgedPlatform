using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TipGuidance;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/admintips")]
public class AdminTipGuidanceController(ITipGuidanceService tipGuidanceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TipGuidanceListItemDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await tipGuidanceService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<TipGuidanceDto>>> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await tipGuidanceService.GetByIdAsync(id, cancellationToken);
        if (data is null)
            return NotFound();

        return Ok(new ApiResponseDto<TipGuidanceDto>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpGet("next-sort-order")]
    public async Task<ActionResult<ApiResponseDto<int>>> GetNextSortOrder(
        CancellationToken cancellationToken = default)
    {
        var data = await tipGuidanceService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponseDto<int>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TipGuidanceDto>>> Create(
        [FromForm] SaveTipGuidanceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var data = await tipGuidanceService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<TipGuidanceDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<TipGuidanceDto>>> Update(
        int id,
        [FromForm] SaveTipGuidanceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var data = await tipGuidanceService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<TipGuidanceDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await tipGuidanceService.DeleteAsync(id, deleteImageFile: true, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "النصيحة غير موجودة",
            Data = deleted
        });
    }
}
