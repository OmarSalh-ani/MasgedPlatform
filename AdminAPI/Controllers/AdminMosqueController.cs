using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Mosques;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminmosque")]
public class AdminMosqueController(IMosqueService mosqueService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<MosqueDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await mosqueService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<MosqueDto>
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
        var data = await mosqueService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponseDto<int>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<MosqueDto>>> Create(
        [FromForm] SaveMosqueRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await mosqueService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<MosqueDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<MosqueDto>>> Update(
        int id,
        [FromForm] SaveMosqueRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await mosqueService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<MosqueDto>
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
        var deleted = await mosqueService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "المسجد غير موجود",
            Data = deleted
        });
    }
}
