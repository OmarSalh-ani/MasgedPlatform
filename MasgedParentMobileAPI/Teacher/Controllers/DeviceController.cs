using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DeviceController(AppDbContext db) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDeviceTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var teacherId = ResolveTeacherId();
        if (teacherId is null)
            return Unauthorized();

        var token = request.FcmToken?.Trim();
        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "رمز الجهاز مطلوب" });

        var platform = string.IsNullOrWhiteSpace(request.Platform)
            ? "unknown"
            : request.Platform.Trim().ToLowerInvariant();

        var existing = await db.TeacherDeviceTokens
            .FirstOrDefaultAsync(t => t.FcmToken == token, cancellationToken);

        if (existing is null)
        {
            db.TeacherDeviceTokens.Add(new TeacherDeviceToken
            {
                TeacherId = teacherId.Value,
                FcmToken = token,
                Platform = platform,
                UpdatedAt = DateTime.UtcNow,
            });
        }
        else
        {
            existing.TeacherId = teacherId.Value;
            existing.Platform = platform;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { message = "تم تسجيل الجهاز" });
    }

    [HttpDelete("unregister")]
    public async Task<IActionResult> Unregister(
        [FromBody] RegisterDeviceTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var teacherId = ResolveTeacherId();
        if (teacherId is null)
            return Unauthorized();

        var token = request.FcmToken?.Trim();
        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "رمز الجهاز مطلوب" });

        var row = await db.TeacherDeviceTokens
            .FirstOrDefaultAsync(
                t => t.FcmToken == token && t.TeacherId == teacherId,
                cancellationToken);

        if (row is not null)
        {
            db.TeacherDeviceTokens.Remove(row);
            await db.SaveChangesAsync(cancellationToken);
        }

        return Ok(new { message = "تم إلغاء تسجيل الجهاز" });
    }

    private int? ResolveTeacherId()
    {
        var raw = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var id) && id > 0 ? id : null;
    }
}
