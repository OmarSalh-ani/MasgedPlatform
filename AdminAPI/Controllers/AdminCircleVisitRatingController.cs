using AdminAPI.DTOs.CircleVisitRating;
using AdminAPI.DTOs.Common;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/admincirclevitrating")]
public class AdminCircleVisitRatingController(ICircleVisitRatingService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CircleVisitRatingListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = CircleVisitRatingService.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var data = await service.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("teachers")]
    public async Task<ActionResult<ApiResponseDto<List<CircleVisitRatingTeacherOptionDto>>>> GetTeachers(
        CancellationToken cancellationToken)
    {
        var data = await service.GetTeachersAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<CircleVisitRatingTeacherOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("circles")]
    public async Task<ActionResult<ApiResponseDto<List<CircleVisitRatingCircleOptionDto>>>> GetCircles(
        [FromQuery] int teacherId,
        CancellationToken cancellationToken)
    {
        var data = await service.GetCirclesAsync(teacherId, cancellationToken);
        return Ok(new ApiResponseDto<List<CircleVisitRatingCircleOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("visit-number")]
    public async Task<ActionResult<ApiResponseDto<CircleVisitRatingVisitNumberDto>>> GetVisitNumber(
        [FromQuery] int teacherId,
        [FromQuery] DateTime visitDate,
        CancellationToken cancellationToken)
    {
        var data = await service.GetVisitNumberAsync(teacherId, visitDate, cancellationToken);
        return Ok(new ApiResponseDto<CircleVisitRatingVisitNumberDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<CircleVisitRatingDetailDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await service.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<CircleVisitRatingDetailDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<CircleVisitRatingDetailDto>>> Create(
        [FromBody] CreateCircleVisitRatingRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await service.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<CircleVisitRatingDetailDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data,
        });
    }

    [HttpGet("{id:int}/export-pdf")]
    public async Task<IActionResult> ExportPdf(int id, CancellationToken cancellationToken)
    {
        var result = await service.ExportPdfAsync(id, cancellationToken);
        return File(result.Bytes, result.ContentType, result.FileName);
    }
}
