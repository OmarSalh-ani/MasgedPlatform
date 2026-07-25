using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Students2;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminstudents2")]
public class AdminStudents2Controller(IStudents2Service students2Service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<Students2ResponseDto>>> GetStudents(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var data = await students2Service.GetStudentsAsync(search ?? string.Empty, cancellationToken);
        return Ok(new ApiResponseDto<Students2ResponseDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }
}
