using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.ContactInfo;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/admincontactinfo")]
public class AdminContactInfoController(IContactInfoService contactInfoService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<ContactInfoDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await contactInfoService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<ContactInfoDto>
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
        var data = await contactInfoService.GetNextSortOrderAsync(cancellationToken);
        return Ok(new ApiResponseDto<int>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ContactInfoDto>>> Create(
        [FromBody] SaveContactInfoRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await contactInfoService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<ContactInfoDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<ContactInfoDto>>> Update(
        int id,
        [FromBody] SaveContactInfoRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await contactInfoService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<ContactInfoDto>
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
        var deleted = await contactInfoService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "بيانات التواصل غير موجودة",
            Data = deleted
        });
    }
}
