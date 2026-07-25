using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.WomansActivities;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminwomansactivity")]
public class AdminWomansActivityController(IWomansActivityService womansActivityService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<WomanActivityDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await womansActivityService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<WomanActivityDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<WomanActivityDto>>> Create(
        [FromBody] SaveWomanActivityRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await womansActivityService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<WomanActivityDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data,
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<WomanActivityDto>>> Update(
        int id,
        [FromBody] SaveWomanActivityRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await womansActivityService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<WomanActivityDto>
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
        var deleted = await womansActivityService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "النشاط غير موجود",
            Data = deleted,
        });
    }
}
