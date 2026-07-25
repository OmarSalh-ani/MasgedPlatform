using System.Security.Claims;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
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
public class PlanLevelsController(AppDbContext db) : ControllerBase
{
    [HttpGet("form-data")]
    public async Task<IActionResult> GetFormData(CancellationToken cancellationToken)
    {
        var surahs = await db.QuranSurahs
            .AsNoTracking()
            .OrderBy(x => x.SortOrder ?? x.Id)
            .Select(x => new IdNameDto { Id = x.Id, Name = "سورة " + x.NameAr })
            .ToListAsync(cancellationToken);

        var jozzList = Enumerable.Range(1, 30)
            .Select(i => new IdNameDto { Id = i, Name = "جزء " + i })
            .ToList();

        return this.ToActionResult(GlobalResponse.Ok(new PlanLevelFormDataDto
        {
            UnitTypes =
            [
                new UnitTypeOptionDto { Value = (byte)PlanUnitType.Page, Label = "صفحة" },
                new UnitTypeOptionDto { Value = (byte)PlanUnitType.QuarterPage, Label = "ربع" },
                new UnitTypeOptionDto { Value = (byte)PlanUnitType.Jozz, Label = "جزء" },
                new UnitTypeOptionDto { Value = (byte)PlanUnitType.Line, Label = "سطر" }
            ],
            Surahs = surahs,
            JozzList = jozzList,
            DefaultFromDate = KuwaitTime.Today.ToString("yyyy-MM-dd"),
            DefaultToDate = KuwaitTime.Today.ToString("yyyy-MM-dd")
        }));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var levels = await db.PlanLevels
            .AsNoTracking()
            .Where(x => x.CreatedByTeacherId == null || x.CreatedByTeacherId == teacherId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        var items = levels.Select(x => MapPlanLevel(x, teacherId)).ToList();
        return this.ToActionResult(GlobalResponse.Ok(items));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var entity = await db.PlanLevels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.CreatedByTeacherId == teacherId, cancellationToken);

        if (entity is null)
            return this.ToActionResult(GlobalResponse.NotFound("مستوى الخطة غير موجود"));

        return this.ToActionResult(GlobalResponse.Ok(MapPlanLevel(entity, teacherId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] SavePlanLevelRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (string.IsNullOrWhiteSpace(request.LevelName) || request.Quantity <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إدخال اسم المستوى والكمية"));

        var entity = new PlanLevel
        {
            LevelName = request.LevelName.Trim(),
            UnitType = request.UnitType,
            Quantity = request.Quantity,
            CreatedAt = KuwaitTime.Now,
            CreatedByTeacherId = teacherId
        };

        db.PlanLevels.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Created(MapPlanLevel(entity, teacherId)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SavePlanLevelRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (string.IsNullOrWhiteSpace(request.LevelName) || request.Quantity <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إدخال اسم المستوى والكمية"));

        var entity = await db.PlanLevels
            .FirstOrDefaultAsync(x => x.Id == id && x.CreatedByTeacherId == teacherId, cancellationToken);

        if (entity is null)
            return this.ToActionResult(GlobalResponse.NotFound("مستوى الخطة غير موجود"));

        entity.LevelName = request.LevelName.Trim();
        entity.UnitType = request.UnitType;
        entity.Quantity = request.Quantity;

        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(MapPlanLevel(entity, teacherId), "تم التعديل بنجاح"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var entity = await db.PlanLevels
            .FirstOrDefaultAsync(x => x.Id == id && x.CreatedByTeacherId == teacherId, cancellationToken);

        if (entity is null)
            return this.ToActionResult(GlobalResponse.NotFound("مستوى الخطة غير موجود"));

        db.PlanLevels.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم الحذف بنجاح"));
    }

    [HttpGet("ready-plans")]
    public async Task<IActionResult> GetReadyPlans(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var items = await BuildReadyPlanDtosAsync(teacherId, cancellationToken);
        return this.ToActionResult(GlobalResponse.Ok(items));
    }

    [HttpGet("ready-plans/{id:int}")]
    public async Task<IActionResult> GetReadyPlanById(int id, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var items = await BuildReadyPlanDtosAsync(teacherId, cancellationToken, id);
        var item = items.FirstOrDefault();
        if (item is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة الجاهزة غير موجودة"));

        return this.ToActionResult(GlobalResponse.Ok(item));
    }

    [HttpPost("ready-plans")]
    public async Task<IActionResult> CreateReadyPlan(
        [FromBody] SaveReadyPlanRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var planLevelId = await ResolvePlanLevelIdAsync(request, teacherId, cancellationToken);
        if (planLevelId <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى تحديد مستوى الخطة"));

        if (!DateTime.TryParse(request.FromDate, out var fromDate))
            fromDate = KuwaitTime.Today;
        if (!DateTime.TryParse(request.ToDate, out var toDate))
            toDate = KuwaitTime.Today;
        if (toDate < fromDate)
            toDate = fromDate;

        var readyPlan = new ReadyPlan
        {
            PlanLevelId = planLevelId,
            FromSurahId = request.FromSurahId,
            ToSurahId = request.ToSurahId,
            FromAyah = request.FromAyah,
            ToAyah = request.ToAyah,
            FromJozz = request.FromJozz,
            ToJozz = request.ToJozz,
            FromDate = fromDate.Date,
            ToDate = toDate.Date,
            CreatedAt = KuwaitTime.Now,
            CreatedByTeacherId = teacherId
        };

        db.ReadyPlans.Add(readyPlan);
        await db.SaveChangesAsync(cancellationToken);

        var dto = (await BuildReadyPlanDtosAsync(teacherId, cancellationToken, readyPlan.Id)).First();
        return this.ToActionResult(GlobalResponse.Created(dto));
    }

    [HttpPut("ready-plans/{id:int}")]
    public async Task<IActionResult> UpdateReadyPlan(
        int id,
        [FromBody] SaveReadyPlanRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var existing = await db.ReadyPlans
            .FirstOrDefaultAsync(x => x.Id == id && x.CreatedByTeacherId == teacherId, cancellationToken);

        if (existing is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة الجاهزة غير موجودة"));

        if (!DateTime.TryParse(request.FromDate, out var fromDate))
            fromDate = existing.FromDate;
        if (!DateTime.TryParse(request.ToDate, out var toDate))
            toDate = existing.ToDate;
        if (toDate < fromDate)
            toDate = fromDate;

        existing.FromSurahId = request.FromSurahId;
        existing.ToSurahId = request.ToSurahId;
        existing.FromAyah = request.FromAyah;
        existing.ToAyah = request.ToAyah;
        existing.FromJozz = request.FromJozz;
        existing.ToJozz = request.ToJozz;
        existing.FromDate = fromDate.Date;
        existing.ToDate = toDate.Date;

        await db.SaveChangesAsync(cancellationToken);

        var dto = (await BuildReadyPlanDtosAsync(teacherId, cancellationToken, id)).First();
        return this.ToActionResult(GlobalResponse.Ok(dto, "تم التعديل بنجاح"));
    }

    [HttpDelete("ready-plans/{id:int}")]
    public async Task<IActionResult> DeleteReadyPlan(int id, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var existing = await db.ReadyPlans
            .FirstOrDefaultAsync(x => x.Id == id && x.CreatedByTeacherId == teacherId, cancellationToken);

        if (existing is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة الجاهزة غير موجودة"));

        db.ReadyPlans.Remove(existing);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم الحذف بنجاح"));
    }

    private async Task<int> ResolvePlanLevelIdAsync(
        SaveReadyPlanRequestDto request,
        int teacherId,
        CancellationToken cancellationToken)
    {
        if (request.PlanLevelId.HasValue && request.PlanLevelId.Value > 0)
            return request.PlanLevelId.Value;

        var levelName = request.LevelName?.Trim() ?? "";
        if (string.IsNullOrEmpty(levelName))
            return 0;

        var existing = await db.PlanLevels
            .FirstOrDefaultAsync(
                pl => pl.LevelName == levelName
                        && (pl.CreatedByTeacherId == null || pl.CreatedByTeacherId == teacherId),
                cancellationToken);

        if (existing is not null)
            return existing.Id;

        if (!request.UnitType.HasValue || !request.Quantity.HasValue || request.Quantity.Value <= 0)
            return 0;

        var newLevel = new PlanLevel
        {
            LevelName = levelName,
            UnitType = request.UnitType.Value,
            Quantity = request.Quantity.Value,
            CreatedAt = KuwaitTime.Now,
            CreatedByTeacherId = teacherId
        };
        db.PlanLevels.Add(newLevel);
        await db.SaveChangesAsync(cancellationToken);
        return newLevel.Id;
    }

    private async Task<List<ReadyPlanDto>> BuildReadyPlanDtosAsync(
        int teacherId,
        CancellationToken cancellationToken,
        int? readyPlanId = null)
    {
        var surahNames = await db.QuranSurahs
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Id, s => "سورة " + s.NameAr, cancellationToken);

        var query = db.ReadyPlans
            .AsNoTracking()
            .Where(x => x.CreatedByTeacherId == null || x.CreatedByTeacherId == teacherId);

        if (readyPlanId.HasValue)
            query = query.Where(x => x.Id == readyPlanId.Value);

        var readyPlans = await query
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        var levelIds = readyPlans.Select(x => x.PlanLevelId).Distinct().ToList();
        var levels = await db.PlanLevels
            .AsNoTracking()
            .Where(pl => levelIds.Contains(pl.Id))
            .ToDictionaryAsync(pl => pl.Id, cancellationToken);

        return readyPlans.Select(x =>
        {
            levels.TryGetValue(x.PlanLevelId, out var level);
            string fromDisp;
            string toDisp;

            if (level is not null && (PlanUnitType)level.UnitType == PlanUnitType.Jozz)
            {
                fromDisp = "جزء " + x.FromJozz;
                toDisp = "جزء " + x.ToJozz;
            }
            else
            {
                surahNames.TryGetValue(x.FromSurahId, out var s1);
                surahNames.TryGetValue(x.ToSurahId, out var s2);
                fromDisp = s1 ?? x.FromSurahId.ToString();
                toDisp = s2 ?? x.ToSurahId.ToString();
            }

            return new ReadyPlanDto
            {
                Id = x.Id,
                PlanLevelId = x.PlanLevelId,
                LevelName = level?.LevelName ?? "—",
                FromSurahName = fromDisp,
                ToSurahName = toDisp,
                FromSurahId = x.FromSurahId,
                ToSurahId = x.ToSurahId,
                FromAyah = x.FromAyah,
                ToAyah = x.ToAyah,
                FromJozz = x.FromJozz,
                ToJozz = x.ToJozz,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                CreatedAt = x.CreatedAt,
                CreatedByTeacherId = x.CreatedByTeacherId,
                CanEdit = x.CreatedByTeacherId == teacherId
            };
        }).ToList();
    }

    private static PlanLevelDto MapPlanLevel(PlanLevel entity, int teacherId) =>
        new()
        {
            Id = entity.Id,
            LevelName = entity.LevelName,
            UnitType = entity.UnitType,
            UnitTypeDisplay = GetUnitDisplay(entity.UnitType),
            Quantity = entity.Quantity,
            CreatedAt = entity.CreatedAt,
            CreatedByTeacherId = entity.CreatedByTeacherId,
            CanEdit = entity.CreatedByTeacherId == teacherId
        };

    private static string GetUnitDisplay(byte unitType) =>
        (PlanUnitType)unitType switch
        {
            PlanUnitType.Page => "صفحة",
            PlanUnitType.QuarterPage => "ربع",
            PlanUnitType.Jozz => "جزء",
            PlanUnitType.Line => "سطر",
            _ => ""
        };

    private bool TryGetTeacherId(out int teacherId)
    {
        teacherId = 0;
        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out teacherId) && teacherId > 0;
    }
}
