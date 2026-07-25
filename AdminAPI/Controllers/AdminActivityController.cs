using AdminAPI.DTOs.Activity;
using AdminAPI.DTOs.Common;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminactivity")]
public class AdminActivityController(IActivityService activityService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<ActivityDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await activityService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<ActivityDto>
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
        var data = await activityService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponseDto<int>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ActivityDto>>> Create(
        [FromForm] SaveActivityRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await activityService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<ActivityDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<ActivityDto>>> Update(
        int id,
        [FromForm] SaveActivityRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await activityService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<ActivityDto>
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
        var deleted = await activityService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "النشاط غير موجود",
            Data = deleted
        });
    }
}
