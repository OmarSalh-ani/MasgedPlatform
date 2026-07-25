using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Students;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminregisterform")]
public class AdminRegisterFormController(IStudentCardPrintService studentCardPrintService) : ControllerBase
{
    [HttpGet("{id:int}/card-print")]
    public async Task<ActionResult<ApiResponseDto<StudentCardPrintDto>>> GetCardPrint(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await studentCardPrintService.GetCardPrintAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<StudentCardPrintDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }
}
