using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MasgedParentMobileAPI.Configuration;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MasgedTeacherMobileAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    AppDbContext db,
    IOptions<TeacherJwtSettings> jwtOptions,
    AccountDeletionService accountDeletionService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى تعبئة جميع الحقول!"));

        var teacher = await db.Teachers
            .Include(t => t.QuranCircles)
            .Where(t => t.Email == request.Email
                        && t.Password == request.Password
                        && t.UsersManage == false)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher is null)
            return this.ToActionResult(GlobalResponse.Unauthorized("خطأ في البريد الألكتروني أو كلمة المرور"));

        var circleId = teacher.QuranCircles.FirstOrDefault()?.Id ?? -1;
        var jwt = jwtOptions.Value;
        var expiresAt = DateTime.UtcNow.AddDays(jwt.ExpiryDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, teacher.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, teacher.Email),
            new("id", teacher.Id.ToString()),
            new("name", teacher.Name),
            new("username", teacher.Email),
            new("isAdmin", teacher.UsersManage.ToString()),
            new("isGirlTeacher", (teacher.IsGirlTeacher ?? false).ToString()),
            new("circleId", circleId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var data = new LoginResponseDto
        {
            Token = tokenString,
            ExpiresAt = expiresAt,
            Id = teacher.Id,
            IsAdmin = teacher.UsersManage,
            Username = teacher.Email,
            IsGirlTeacher = teacher.IsGirlTeacher ?? false,
            CircleId = circleId,
            Name = teacher.Name
        };

        return this.ToActionResult(GlobalResponse.Ok(data, "تم تسجيل الدخول بنجاح"));
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return this.ToActionResult(GlobalResponse.Ok(message: "تم تسجيل الخروج بنجاح"));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إدخال كلمة المرور الجديدة"));

        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var teacherId) || teacherId <= 0)
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var teacher = await db.Teachers
            .FirstOrDefaultAsync(t => t.Id == teacherId && !t.UsersManage, cancellationToken);

        if (teacher is null)
            return this.ToActionResult(GlobalResponse.NotFound("المعلم غير موجود"));

        teacher.Password = request.NewPassword.Trim();
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم تغيير كلمة المرور بنجاح. يرجى تسجيل الدخول مرة أخرى"));
    }

    [Authorize]
    [HttpPost("delete-account")]
    public async Task<IActionResult> DeleteAccount(
        [FromBody] DeleteAccountRequestDto request,
        CancellationToken cancellationToken)
    {
        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var teacherId) || teacherId <= 0)
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var (success, message) = await accountDeletionService.DeleteTeacherAccountAsync(
            teacherId,
            request.Password ?? string.Empty,
            cancellationToken);

        if (!success)
            return this.ToActionResult(GlobalResponse.BadRequest(message));

        return this.ToActionResult(GlobalResponse.Ok(message: message));
    }
}
