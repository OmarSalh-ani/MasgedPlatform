using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Competitions;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/admincompetition")]
public class AdminCompetitionController(ICompetitionService competitionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CompetitionListItemDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await competitionService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<CompetitionDto>>> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await competitionService.GetByIdAsync(id, cancellationToken);
        if (data is null)
            return NotFound();

        return Ok(new ApiResponseDto<CompetitionDto>
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
        var data = await competitionService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponseDto<int>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<CompetitionDto>>> Create(
        [FromForm] SaveCompetitionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var data = await competitionService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<CompetitionDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<CompetitionDto>>> Update(
        int id,
        [FromForm] SaveCompetitionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var data = await competitionService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<CompetitionDto>
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
        var deleted = await competitionService.DeleteAsync(id, deleteImageFile: true, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "المسابقة غير موجودة",
            Data = deleted
        });
    }
}
