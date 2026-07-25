using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.ParentsFollowup;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/adminparentsfollowup")]
public class AdminParentsFollowupController(IParentsFollowupService service) : ControllerBase
{
    [HttpGet("{studentId:int}")]
    public async Task<ActionResult<ApiResponseDto<ParentsFollowupDto>>> GetByStudentId(
        int studentId,
        CancellationToken cancellationToken)
    {
        var data = await service.GetByStudentIdAsync(studentId, cancellationToken);
        if (data is null)
        {
            return NotFound(new ApiResponseDto<ParentsFollowupDto>
            {
                Success = false,
                Message = "الطالب غير موجود",
            });
        }

        return Ok(new ApiResponseDto<ParentsFollowupDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPut("{studentId:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Submit(
        int studentId,
        [FromForm] SaveParentsFollowupRequestDto request,
        CancellationToken cancellationToken)
    {
        await service.SubmitAsync(studentId, request, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = true,
            Message = "تم الإرسال بنجاح",
            Data = true,
        });
    }
}
