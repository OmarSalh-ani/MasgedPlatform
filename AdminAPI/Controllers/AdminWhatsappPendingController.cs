using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.WhatsappPending;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminwhatsapppending")]
public class AdminWhatsappPendingController(IWhatsappPendingService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<WhatsappPendingMessageDto>>>> GetList(
        CancellationToken cancellationToken)
    {
        var data = await service.GetListAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<WhatsappPendingMessageDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("delete-selected")]
    public async Task<ActionResult<ApiResponseDto<int>>> DeleteSelected(
        [FromBody] DeleteWhatsappPendingRequestDto request,
        CancellationToken cancellationToken)
    {
        var count = await service.DeleteSelectedAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<int> { Success = true, Message = "تم الحذف", Data = count });
    }

    [HttpDelete]
    public async Task<ActionResult<ApiResponseDto<int>>> DeleteAll(CancellationToken cancellationToken)
    {
        var count = await service.DeleteAllAsync(cancellationToken);
        return Ok(new ApiResponseDto<int> { Success = true, Message = "تم الحذف", Data = count });
    }
}
