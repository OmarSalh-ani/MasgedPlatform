using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/parent-schedule")]
[Authorize]
public sealed class ParentScheduleController(NewMasgedTeacherAPIDBContext db, IWorkDayService workDayService)
    : ControllerBase
{
    /// <summary>Saturday-first display order for Arabic weekday labels.</summary>
    private static readonly int[] DisplayOrder = [6, 0, 1, 2, 3, 4, 5];

    private static string ArabicDayName(int dow)
    {
        return dow switch
        {
            0 => "الأحد",
            1 => "الاثنين",
            2 => "الثلاثاء",
            3 => "الأربعاء",
            4 => "الخميس",
            5 => "الجمعة",
            6 => "السبت",
            _ => $"يوم ({dow})",
        };
    }

    [HttpGet]
    public async Task<ActionResult<List<ParentScheduleSlotDto>>> Get(CancellationToken cancellationToken)
    {
        var fp = User.FindFirstValue("fatherPhone");
        if (string.IsNullOrEmpty(fp)) return Unauthorized();

        var variants = PhoneNormalizer.GetVariants(fp).ToList();

        var students = await db.RegisterForms
            .AsNoTracking()
            .Include(r => r.QuranCircle)
            .Where(r =>
                variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2))
            .ToListAsync(cancellationToken);

        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);
        var workDaySet = workDayNumbers.ToHashSet();
        var weekdayNames = DisplayOrder
            .Where(d => workDaySet.Contains(d))
            .Select(ArabicDayName)
            .ToList();

        var result = new List<ParentScheduleSlotDto>();

        foreach (var student in students)
        {
            if (student.QuranCircleId is null || student.QuranCircleId <= 0) continue;

            result.Add(new ParentScheduleSlotDto
            {
                StudentId = student.Id,
                StudentName =
                    student.StudentName ??
                    student.FullName ??
                    string.Empty,
                CircleName =
                    student.QuranCircle?.Name ?? "الحلقة",
                WeekdaysArabic = weekdayNames,
            });
        }

        return Ok(result);
    }
}
