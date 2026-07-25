using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/parent/followup")]
[Authorize]
public class ParentFollowupController : ControllerBase
{
    private readonly StudentService _studentService;

    public ParentFollowupController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<ActionResult<ParentFollowupDto>> Get()
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var dto = await _studentService.GetParentFollowupAsync(fatherPhone);
        return Ok(dto);
    }

    [HttpPut]
    public async Task<ActionResult<ParentFollowupDto>> Update([FromBody] UpdateParentFollowupRequest request)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        try
        {
            var dto = await _studentService.UpdateParentFollowupAsync(fatherPhone, request);
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private string? GetFatherPhone() => User.FindFirstValue("fatherPhone");
}
