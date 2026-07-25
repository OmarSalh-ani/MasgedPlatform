using System.Security.Claims;
using MasgedParentMobileAPI.Services;
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
public class HomeController(AppDbContext db, IWorkDayService workDayService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHome(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var circleName = await db.QuranCircles
            .AsNoTracking()
            .Where(c => c.Id == circleId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var isWorkDayToday = await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken);

        var statistics = await StudentHomeHelper.ComputeCircleStatisticsAsync(
            db, circleId, isWorkDayToday, cancellationToken);

        var planLevels = await db.PlanLevels
            .AsNoTracking()
            .OrderBy(x => x.LevelName)
            .Select(x => new IdNameDto { Id = x.Id, Name = x.LevelName })
            .ToListAsync(cancellationToken);

        var unreadAdminNotes = await db.TeachersAdminNotes
            .CountAsync(n => n.TeacherId == teacherId && !n.IsRead, cancellationToken);

        var students = await StudentHomeHelper.LoadCircleStudentsAsync(
            db, circleId, isWorkDayToday, cancellationToken, search);

        return this.ToActionResult(GlobalResponse.Ok(new GetHomeResponseDto
        {
            TeacherName = User.FindFirstValue("name") ?? "المعلم",
            CircleName = circleName,
            IsWorkDayToday = isWorkDayToday,
            Statistics = statistics,
            PlanLevels = planLevels,
            UnreadAdminNotesCount = unreadAdminNotes,
            Students = students
        }));
    }

    private bool TryGetTeacherContext(out int teacherId, out int circleId)
    {
        teacherId = 0;
        circleId = 0;
        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(idClaim, out teacherId) && teacherId > 0
               && int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }
}
