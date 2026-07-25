using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.QuranCircles;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminqurancircle")]
public class AdminQuranCircleController(IQuranCircleService quranCircleService) : ControllerBase
{
    [HttpGet("teachers")]
    public async Task<ActionResult<ApiResponseDto<List<TeacherOptionDto>>>> GetTeachers(
        CancellationToken cancellationToken)
    {
        var data = await quranCircleService.GetTeachersAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<TeacherOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<QuranCircleDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await quranCircleService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<QuranCircleDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<QuranCircleDto>>> Create(
        [FromBody] SaveQuranCircleRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await quranCircleService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<QuranCircleDto>
        {
            Success = true,
            Message = "تم إضافة الحلقة بنجاح",
            Data = data,
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<QuranCircleDto>>> Update(
        int id,
        [FromBody] SaveQuranCircleRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await quranCircleService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<QuranCircleDto>
        {
            Success = true,
            Message = "تم تحديث الحلقة بنجاح",
            Data = data,
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await quranCircleService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم حذف الحلقة بنجاح" : "الحلقة غير موجودة",
            Data = deleted,
        });
    }
}
