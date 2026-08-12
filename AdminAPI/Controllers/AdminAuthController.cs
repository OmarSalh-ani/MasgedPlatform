using AdminAPI.DTOs.Auth;
using AdminAPI.DTOs.Common;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminauth")]
public class AdminAuthController(IAuthService authService, ICurrentUserContext currentUser) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var (success, message, data) = await authService.LoginAsync(request, cancellationToken);

        return Ok(new ApiResponseDto<LoginResponseDto>
        {
            Success = success,
            Message = message,
            Data = data
        });
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword(
        [FromBody] ChangePasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        var (success, message) = await authService.ChangePasswordAsync(
            currentUser.TeacherId,
            request,
            cancellationToken);

        return Ok(new ApiResponseDto<bool>
        {
            Success = success,
            Message = message,
            Data = success
        });
    }

    [Authorize]
    [HttpGet("session")]
    public ActionResult<ApiResponseDto<AdminSessionDto>> GetSession()
    {
        if (currentUser.TeacherId <= 0)
        {
            return Unauthorized(new ApiResponseDto<AdminSessionDto>
            {
                Success = false,
                Message = "غير مصرح",
            });
        }

        return Ok(new ApiResponseDto<AdminSessionDto>
        {
            Success = true,
            Message = "OK",
            Data = new AdminSessionDto
            {
                Id = currentUser.TeacherId,
                Username = User.Identity?.Name ?? string.Empty,
                IsAdmin = currentUser.IsAdmin,
                IsGirlTeacher = currentUser.IsGirlTeacher,
                IsViewOnly = !currentUser.CanModify,
                IsSupervisor = currentUser.IsSupervisor,
            },
        });
    }
}
