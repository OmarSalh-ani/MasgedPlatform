using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.ContactInfo;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/admincontactinfos")]
public class AdminContactInfosController(IContactInfoService contactInfoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ContactInfoListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await contactInfoService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
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
