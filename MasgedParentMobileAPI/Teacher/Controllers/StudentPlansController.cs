using System.Globalization;
using System.Security.Claims;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Enums;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentPlansController(AppDbContext db, IWorkDayService workDayService) : ControllerBase
{
    [HttpGet("assign-form")]
    public async Task<IActionResult> GetAssignPlanFormData(
        [FromQuery] int? studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على حلقتك. يرجى تسجيل الدخول مرة أخرى."));

        var surahs = await db.QuranSurahs
            .AsNoTracking()
            .OrderBy(x => x.SortOrder ?? x.Id)
            .Select(x => new IdNameDto { Id = x.Id, Name = "سورة " + x.NameAr })
            .ToListAsync(cancellationToken);

        var jozzList = Enumerable.Range(1, 30)
            .Select(i => new IdNameDto { Id = i, Name = "جزء " + i })
            .ToList();

        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var planLevels = await db.PlanLevels
            .AsNoTracking()
            .Where(x => x.CreatedByTeacherId == null || x.CreatedByTeacherId == teacherId)
            .Select(x => new PlanLevelPickDto
            {
                Id = x.Id,
                LevelName = x.LevelName,
                UnitType = x.UnitType,
                UsesJozzInput = x.UnitType == (byte)PlanUnitType.Jozz
                    || (x.LevelName.Contains("جزء")
                        && !x.LevelName.Contains("صفحة")
                        && !x.LevelName.Contains("سطر"))
            })
            .ToListAsync(cancellationToken);

        var studentsQuery = db.RegisterForms
            .AsNoTracking()
            .Where(s => s.QuranCircleId == circleId);

        if (studentId.HasValue && studentId.Value > 0)
            studentsQuery = studentsQuery.Where(s => s.Id == studentId.Value);

        var students = await studentsQuery
            .OrderBy(s => s.FullName ?? s.StudentName ?? "")
            .Select(s => new IdNameDto
            {
                Id = s.Id,
                Name = s.FullName ?? s.StudentName ?? "—"
            })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new AssignPlanFormDataDto
        {
            PlanLevels = planLevels,
            Surahs = surahs,
            JozzList = jozzList,
            Students = students
        }));
    }

    [HttpGet("surahs/{surahId:int}/ayahs")]
    public async Task<IActionResult> GetAyahsBySurah(int surahId, CancellationToken cancellationToken)
    {
        var ayahs = await db.QuranAyahs
            .AsNoTracking()
            .Where(x => x.SurahId == surahId)
            .OrderBy(x => x.AyahNumber)
            .Select(x => new { ayahNumber = x.AyahNumber })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(ayahs));
    }

    [HttpGet("circle-days-count")]
    public async Task<IActionResult> GetCircleDaysCount(
        [FromQuery] string startDate,
        [FromQuery] string endDate,
        CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(startDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start)
            || !DateTime.TryParse(endDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
        {
            return this.ToActionResult(GlobalResponse.BadRequest("تواريخ غير صالحة"));
        }

        if (end < start)
            end = start;

        if (!TryGetCircleId(out _))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على حلقتك. يرجى تسجيل الدخول مرة أخرى."));

        var count = await workDayService.CountWorkDaysInRangeAsync(start, end, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new { count }));
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignPlan(
        [FromBody] AssignPlanRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على حلقتك. يرجى تسجيل الدخول مرة أخرى."));

        if (request.PlanLevelId <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى اختيار مستوى الخطة"));

        var validStudentIds = await db.RegisterForms
            .AsNoTracking()
            .Where(s => request.StudentIds.Contains(s.Id) && s.QuranCircleId == circleId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        if (validStudentIds.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى اختيار طلاب"));

        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);

        var result = await AssignPlanHelper.AssignPlanAsync(
            db,
            validStudentIds,
            request.PlanLevelId,
            request.FromSurahId,
            request.ToSurahId,
            request.FromJozz,
            request.ToJozz,
            request.FromDate,
            request.ToDate,
            request.PlanType,
            request.FromAyahNumber,
            request.ToAyahNumber,
            circleId,
            workDayNumbers,
            cancellationToken);

        if (!result.Success)
            return this.ToActionResult(GlobalResponse.BadRequest(result.Error ?? "حدث خطأ"));

        return this.ToActionResult(GlobalResponse.Ok(message: "تم تحديد وإنشاء الخطة بنجاح"));
    }

    private bool TryGetCircleId(out int circleId)
    {
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }

    private bool TryGetTeacherId(out int teacherId)
    {
        teacherId = 0;
        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out teacherId) && teacherId > 0;
    }
}
