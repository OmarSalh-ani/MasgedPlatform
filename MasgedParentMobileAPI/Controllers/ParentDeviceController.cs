using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/parent/device")]
[Authorize]
public sealed class ParentDeviceController : ControllerBase
{
    private readonly NewMasgedTeacherAPIDBContext _db;

    public ParentDeviceController(NewMasgedTeacherAPIDBContext db)
    {
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDeviceTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var phone = ResolveParentPhone();
        if (phone is null)
            return Unauthorized();

        var token = request.FcmToken?.Trim();
        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "رمز الجهاز مطلوب" });

        var platform = string.IsNullOrWhiteSpace(request.Platform)
            ? "unknown"
            : request.Platform.Trim().ToLowerInvariant();

        var existing = await _db.ParentDeviceTokens
            .FirstOrDefaultAsync(t => t.FcmToken == token, cancellationToken);

        if (existing is null)
        {
            _db.ParentDeviceTokens.Add(new ParentDeviceToken
            {
                ParentPhone = phone,
                FcmToken = token,
                Platform = platform,
                UpdatedAt = DateTime.UtcNow,
            });
        }
        else
        {
            existing.ParentPhone = phone;
            existing.Platform = platform;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(new { message = "تم تسجيل الجهاز" });
    }

    [HttpDelete("unregister")]
    public async Task<IActionResult> Unregister(
        [FromBody] RegisterDeviceTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var phone = ResolveParentPhone();
        if (phone is null)
            return Unauthorized();

        var token = request.FcmToken?.Trim();
        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "رمز الجهاز مطلوب" });

        var row = await _db.ParentDeviceTokens
            .FirstOrDefaultAsync(
                t => t.FcmToken == token && t.ParentPhone == phone,
                cancellationToken);

        if (row is not null)
        {
            _db.ParentDeviceTokens.Remove(row);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return Ok(new { message = "تم إلغاء تسجيل الجهاز" });
    }

    private string? ResolveParentPhone()
    {
        var raw = User.FindFirstValue("fatherPhone");
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        return PhoneNormalizer.ToCanonical(raw);
    }
}
