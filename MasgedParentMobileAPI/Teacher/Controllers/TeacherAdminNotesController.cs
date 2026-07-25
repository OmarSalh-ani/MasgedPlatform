using System.Globalization;
using System.Security.Claims;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TeacherAdminNotesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotes(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var notes = await db.TeachersAdminNotes
            .AsNoTracking()
            .Where(n => n.TeacherId == teacherId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new
            {
                n.Id,
                n.Note,
                n.CreatedAt,
                n.IsRead,
                n.ReadTime
            })
            .ToListAsync(cancellationToken);

        var result = notes.Select(n => new TeacherAdminNoteDto
        {
            Id = n.Id,
            Note = n.Note,
            CreatedAt = n.CreatedAt,
            CreatedAtFormatted = n.CreatedAt.ToString(
                "dd/MM/yyyy hh:mm tt",
                CultureInfo.InvariantCulture),
            IsRead = n.IsRead,
            ReadTime = n.ReadTime,
            ReadTimeFormatted = n.ReadTime.HasValue
                ? n.ReadTime.Value.ToString(
                    "dd/MM/yyyy hh:mm tt",
                    CultureInfo.InvariantCulture)
                : null
        }).ToList();

        return this.ToActionResult(GlobalResponse.Ok(result));
    }

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var unreadNotes = await db.TeachersAdminNotes
            .Where(n => n.TeacherId == teacherId && !n.IsRead)
            .ToListAsync(cancellationToken);

        var now = KuwaitTime.Now;
        foreach (var note in unreadNotes)
        {
            note.IsRead = true;
            note.ReadTime = now;
        }

        if (unreadNotes.Count > 0)
            await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(
            new { markedCount = unreadNotes.Count },
            "تم تحديث حالة الملاحظات"));
    }

    private bool TryGetTeacherId(out int teacherId)
    {
        teacherId = 0;
        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out teacherId) && teacherId > 0;
    }
}
