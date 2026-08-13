using System.Globalization;
using System.Security.Claims;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentPlan2Controller(AppDbContext db, IWorkDayService workDayService) : ControllerBase
{
    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var students = await db.RegisterForms
            .AsNoTracking()
            .Where(x => x.QuranCircleId == circleId)
            .OrderBy(x => x.FullName ?? x.StudentName)
            .Select(x => new StudentPlan2StudentListItemDto
            {
                Id = x.Id,
                Name = x.FullName ?? x.StudentName ?? "—"
            })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(students));
    }

    [HttpGet("form-data")]
    public async Task<IActionResult> GetFormData(CancellationToken cancellationToken)
    {
        var surahs = await db.QuranSurahs
            .AsNoTracking()
            .OrderBy(x => x.SortOrder ?? x.Id)
            .Select(x => new IdNameDto { Id = x.Id, Name = "سورة " + x.NameAr })
            .ToListAsync(cancellationToken);

        var planLevels = await db.PlanLevels
            .AsNoTracking()
            .Where(x => x.CreatedByTeacherId == null
                        || x.CreatedByTeacherId == GetTeacherId())
            .Select(x => new PlanLevelPickDto
            {
                Id = x.Id,
                LevelName = x.LevelName,
                UnitType = x.UnitType,
                UsesJozzInput = x.UnitType == (byte)Enums.PlanUnitType.Jozz
            })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new { surahs, planLevels }));
    }

    [HttpPost("bulk-plans")]
    public async Task<IActionResult> BulkAssignPlans(
        [FromBody] BulkAssignPlanRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (request.StudentIds.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى اختيار طالب واحد على الأقل"));

        List<PlanRowInputDto> rows = request.Plan.Rows ?? [];
        if (rows.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إضافة سطر واحد على الأقل"));

        var planStart = request.Plan.PlanStartDate?.Date ?? KuwaitTime.Today;
        var planEnd = request.Plan.PlanEndDate?.Date ?? planStart;
        var now = KuwaitTime.Now;
        var today = KuwaitTime.Today;

        var distinctIds = request.StudentIds.Where(id => id > 0).Distinct().ToList();
        var studentsInCircle = await db.RegisterForms
            .AsNoTracking()
            .Where(s => s.QuranCircleId == circleId && distinctIds.Contains(s.Id))
            .Select(s => new { s.Id, Name = s.FullName ?? s.StudentName ?? "—" })
            .ToDictionaryAsync(s => s.Id, s => s.Name, cancellationToken);

        var results = new List<BulkAssignPlanStudentResultDto>();

        foreach (var studentId in distinctIds)
        {
            if (!studentsInCircle.TryGetValue(studentId, out var studentName))
            {
                results.Add(new BulkAssignPlanStudentResultDto
                {
                    StudentId = studentId,
                    StudentName = "—",
                    Success = false,
                    Message = "الطالب غير موجود في الحلقة"
                });
                continue;
            }

            try
            {
                int planId;
                string message;

                if (request.AddToExistingPlan)
                {
                    var studentPlans = await db.StudentPlans
                        .Where(p => p.StudentId == studentId && !p.IsArchived)
                        .OrderBy(p => p.PlanFromDate)
                        .ToListAsync(cancellationToken);

                    var existingPlan = studentPlans
                        .FirstOrDefault(p => today >= p.PlanFromDate && today <= p.PlanToDate)
                        ?? studentPlans.FirstOrDefault();

                    if (existingPlan is not null)
                    {
                        await AddPlanRowsAsync(
                            studentId,
                            existingPlan.Id,
                            rows,
                            planStart,
                            planEnd,
                            now,
                            cancellationToken);
                        await db.SaveChangesAsync(cancellationToken);
                        planId = existingPlan.Id;
                        message = "تمت الإضافة للخطة الحالية";
                    }
                    else
                    {
                        planId = await CreatePlanForStudentAsync(
                            studentId,
                            request.Plan,
                            rows,
                            planStart,
                            planEnd,
                            now,
                            cancellationToken);
                        message = "تم إنشاء خطة جديدة";
                    }
                }
                else
                {
                    planId = await CreatePlanForStudentAsync(
                        studentId,
                        request.Plan,
                        rows,
                        planStart,
                        planEnd,
                        now,
                        cancellationToken);
                    message = "تم إنشاء الخطة بنجاح";
                }

                results.Add(new BulkAssignPlanStudentResultDto
                {
                    StudentId = studentId,
                    StudentName = studentName,
                    Success = true,
                    PlanId = planId,
                    Message = message
                });
            }
            catch (Exception ex)
            {
                results.Add(new BulkAssignPlanStudentResultDto
                {
                    StudentId = studentId,
                    StudentName = studentName,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        var response = new BulkAssignPlanResponseDto
        {
            SuccessCount = results.Count(r => r.Success),
            FailedCount = results.Count(r => !r.Success),
            Results = results
        };

        var statusMessage = response.FailedCount == 0
            ? $"تم تعيين الخطة لـ {response.SuccessCount} طالب"
            : $"نجح {response.SuccessCount} — فشل {response.FailedCount}";

        return this.ToActionResult(GlobalResponse.Ok(response, statusMessage));
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

    [HttpGet("{studentId:int}")]
    public async Task<IActionResult> GetStudentOverview(int studentId, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var student = await GetStudentInCircleAsync(studentId, circleId, cancellationToken);
        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var today = KuwaitTime.Today;
        var plans = await db.StudentPlans
            .AsNoTracking()
            .Where(p => p.StudentId == studentId && !p.IsArchived)
            .OrderBy(p => p.PlanFromDate)
            .ToListAsync(cancellationToken);

        var summaries = plans.Select(p => new StudentPlanSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            PlanFromDate = p.PlanFromDate,
            PlanToDate = p.PlanToDate,
            IsCurrent = today >= p.PlanFromDate && today <= p.PlanToDate
        }).ToList();

        var currentPlan = plans.FirstOrDefault(p => today >= p.PlanFromDate && today <= p.PlanToDate)
                          ?? plans.FirstOrDefault();

        return this.ToActionResult(GlobalResponse.Ok(new StudentPlan2StudentOverviewDto
        {
            StudentId = student.Id,
            StudentName = student.FullName ?? student.StudentName,
            IsNewPlanMode = plans.Count == 0,
            SuggestedPlanId = currentPlan?.Id,
            Plans = summaries
        }));
    }

    [HttpGet("{studentId:int}/plans/{planId:int}")]
    public async Task<IActionResult> GetPlanDetail(
        int studentId,
        int planId,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var student = await GetStudentInCircleAsync(studentId, circleId, cancellationToken);
        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var plan = await db.StudentPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == planId && p.StudentId == studentId && !p.IsArchived, cancellationToken);

        if (plan is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة غير موجودة"));

        var circleDayNumbers = await GetWorkDayNumbersAsync(cancellationToken);
        var detail = await StudentPlan2Helper.BuildPlanDetailAsync(
            db, student, plan, circleDayNumbers, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(detail));
    }

    [HttpGet("{studentId:int}/plans/{planId:int}/progress")]
    public async Task<IActionResult> GetPlanProgress(
        int studentId,
        int planId,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentInCircleAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var memStatuses = await db.StudentPlanMemorizings
            .AsNoTracking()
            .Where(x => x.StudentId == studentId && x.PlanId == planId)
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        var revStatuses = await db.StudentPlanRevises
            .AsNoTracking()
            .Where(x => x.StudentId == studentId && x.PlanId == planId)
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        var allStatuses = memStatuses.Concat(revStatuses).ToList();
        if (allStatuses.Count == 0)
            return this.ToActionResult(GlobalResponse.Ok(new PlanProgressDto()));

        var plan = await db.StudentPlans.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);
        var circleDayNumbers = await GetWorkDayNumbersAsync(cancellationToken);

        var progress = StudentPlan2Helper.BuildProgress(
            allStatuses,
            plan?.PlanFromDate ?? KuwaitTime.Today,
            plan?.PlanToDate ?? KuwaitTime.Today,
            circleDayNumbers);

        return this.ToActionResult(GlobalResponse.Ok(progress));
    }

    [HttpPost("{studentId:int}/plans")]
    public async Task<IActionResult> CreatePlan(
        int studentId,
        [FromBody] SavePlanRowsRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var rows = request.Rows ?? [];
        if (rows.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إضافة سطر واحد على الأقل"));

        var planStart = request.PlanStartDate?.Date ?? KuwaitTime.Today;
        var planEnd = request.PlanEndDate?.Date ?? planStart;
        var now = KuwaitTime.Now;

        var plan = new StudentPlan
        {
            StudentId = studentId,
            Name = string.IsNullOrWhiteSpace(request.PlanName)
                ? "خطة جديدة " + planStart.ToString("yyyy-MM-dd")
                : request.PlanName.Trim(),
            PlanFromDate = planStart,
            PlanToDate = planEnd,
            CreatedAt = now
        };
        db.StudentPlans.Add(plan);
        await db.SaveChangesAsync(cancellationToken);

        await AddPlanRowsAsync(studentId, plan.Id, rows, planStart, planEnd, now, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Created(new { planId = plan.Id }));
    }

    [HttpPost("{studentId:int}/plans/{planId:int}/rows")]
    public async Task<IActionResult> AddPlanRows(
        int studentId,
        int planId,
        [FromBody] SavePlanRowsRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var plan = await db.StudentPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.StudentId == studentId && !p.IsArchived, cancellationToken);
        if (plan is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة غير موجودة"));

        var rows = request.Rows ?? [];
        if (rows.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إضافة سطر واحد على الأقل"));

        var planStart = request.PlanStartDate?.Date ?? plan.PlanFromDate;
        var planEnd = request.PlanEndDate?.Date ?? plan.PlanToDate;
        if (planEnd < planStart)
            planEnd = planStart;

        plan.PlanFromDate = planStart;
        plan.PlanToDate = planEnd;
        var now = KuwaitTime.Now;

        await AddPlanRowsAsync(studentId, planId, rows, planStart, planEnd, now, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم حفظ الخطة بنجاح"));
    }

    [HttpPut("{studentId:int}/plans/{planId:int}/dates")]
    public async Task<IActionResult> UpdatePlanDates(
        int studentId,
        int planId,
        [FromBody] UpdatePlanDatesRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var plan = await db.StudentPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.StudentId == studentId && !p.IsArchived, cancellationToken);
        if (plan is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة غير موجودة"));

        var planStart = request.PlanStartDate.Date;
        var planEnd = request.PlanEndDate.Date;
        if (planEnd < planStart)
            planEnd = planStart;

        plan.PlanFromDate = planStart;
        plan.PlanToDate = planEnd;
        await db.SaveChangesAsync(cancellationToken);

        var memStatuses = await db.StudentPlanMemorizings
            .AsNoTracking()
            .Where(x => x.StudentId == studentId && x.PlanId == planId)
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        var revStatuses = await db.StudentPlanRevises
            .AsNoTracking()
            .Where(x => x.StudentId == studentId && x.PlanId == planId)
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        var allStatuses = memStatuses.Concat(revStatuses).ToList();
        var circleDayNumbers = await GetWorkDayNumbersAsync(cancellationToken);
        var progress = StudentPlan2Helper.BuildProgress(
            allStatuses,
            plan.PlanFromDate,
            plan.PlanToDate,
            circleDayNumbers);

        return this.ToActionResult(GlobalResponse.Ok(progress, "تم حفظ التواريخ بنجاح"));
    }

    [HttpPost("{studentId:int}/plans/{planId:int}/revise-rows")]
    public async Task<IActionResult> AddReviseRows(
        int studentId,
        int planId,
        [FromBody] SaveReviseRowsRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var plan = await db.StudentPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.StudentId == studentId && !p.IsArchived, cancellationToken);
        if (plan is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة غير موجودة"));

        var reviseDate = DateTime.TryParse(request.ReviseDate, out var rd)
            ? rd.Date
            : KuwaitTime.Today;

        var level = await db.StudentPlanRevises
            .Where(x => x.PlanId == planId)
            .Select(x => x.MemorizationLevel)
            .FirstOrDefaultAsync(cancellationToken)
            ?? await db.StudentPlanMemorizings
                .Where(x => x.PlanId == planId)
                .Select(x => x.MemorizationLevel)
                .FirstOrDefaultAsync(cancellationToken)
            ?? "—";

        var planEndDate = plan.PlanToDate;
        var now = KuwaitTime.Now;
        var saved = 0;

        foreach (var row in request.Rows)
        {
            if (row.SurahId <= 0 || row.FromAyahNumber <= 0 || row.ToAyahNumber <= 0)
                continue;

            db.StudentPlanRevises.Add(new StudentPlanRevise
            {
                StudentId = studentId,
                PlanId = planId,
                MemorizationLevel = level,
                SurahId = row.SurahId,
                FromAyahNumber = row.FromAyahNumber,
                ToAyahNumber = row.ToAyahNumber,
                PlanDate = reviseDate,
                PlanEndDate = planEndDate != default ? planEndDate : reviseDate,
                CreatedAt = now,
                Status = PlanRowStatus.Pending
            });
            saved++;
        }

        if (saved == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إضافة سطر مراجعة واحد على الأقل"));

        await db.SaveChangesAsync(cancellationToken);
        return this.ToActionResult(GlobalResponse.Ok(message: "تم حفظ المراجعة بنجاح"));
    }

    [HttpPost("{studentId:int}/plans/{planId:int}/assign-revise")]
    public async Task<IActionResult> AssignReviseToExistingPlan(
        int studentId,
        int planId,
        [FromBody] AssignReviseToPlanRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);

        var result = await AssignPlanHelper.AssignReviseToExistingPlanAsync(
            db,
            studentId,
            planId,
            request.PlanLevelId,
            request.FromSurahId,
            request.ToSurahId,
            request.FromJozz,
            request.ToJozz,
            request.FromDate,
            request.ToDate,
            request.FromAyahNumber,
            request.ToAyahNumber,
            circleId,
            workDayNumbers,
            cancellationToken);

        if (!result.Success)
            return this.ToActionResult(GlobalResponse.BadRequest(result.Error ?? "حدث خطأ"));

        return this.ToActionResult(GlobalResponse.Ok(message: "تم توزيع المراجعة بنجاح"));
    }

    [HttpPut("rows/{rowKey}")]
    public async Task<IActionResult> UpdateRow(
        string rowKey,
        [FromQuery] int studentId,
        [FromBody] UpdatePlanRowRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var validationError = StudentPlanRowUpdater.ValidateRequest(request);
        if (validationError is not null)
            return this.ToActionResult(GlobalResponse.BadRequest(validationError));

        var planId = await ResolvePlanIdFromRowKeyAsync(studentId, rowKey, cancellationToken);
        if (planId <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("مفتاح السطر غير صالح"));

        var plan = await db.StudentPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == planId && p.StudentId == studentId && !p.IsArchived, cancellationToken);
        if (plan is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة غير موجودة"));

        var planStart = plan.PlanFromDate;
        var planEnd = plan.PlanToDate;
        if (planEnd < planStart)
            planEnd = planStart;

        var updated = StudentPlanRowUpdater.UpdateRow(
            db,
            studentId,
            rowKey,
            request,
            planStart,
            planEnd,
            KuwaitTime.Now);

        if (!updated)
            return this.ToActionResult(GlobalResponse.NotFound("السجل غير موجود"));

        await db.SaveChangesAsync(cancellationToken);
        return this.ToActionResult(GlobalResponse.Ok(message: "تم التحديث بنجاح"));
    }

    [HttpDelete("rows/{rowKey}")]
    public async Task<IActionResult> DeleteRow(
        string rowKey,
        [FromQuery] int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        if (rowKey.StartsWith("memorizing_") && int.TryParse(rowKey["memorizing_".Length..], out var memId))
        {
            var ent = await db.StudentPlanMemorizings.FindAsync([memId], cancellationToken);
            if (ent is not null)
            {
                db.StudentPlanMemorizings.Remove(ent);
                await db.SaveChangesAsync(cancellationToken);
            }
            return this.ToActionResult(GlobalResponse.Ok(message: "تم الحذف بنجاح"));
        }

        if (rowKey.StartsWith("revise_") && int.TryParse(rowKey["revise_".Length..], out var revId))
        {
            var ent = await db.StudentPlanRevises.FindAsync([revId], cancellationToken);
            if (ent is not null)
            {
                db.StudentPlanRevises.Remove(ent);
                await db.SaveChangesAsync(cancellationToken);
            }
            return this.ToActionResult(GlobalResponse.Ok(message: "تم الحذف بنجاح"));
        }

        return this.ToActionResult(GlobalResponse.BadRequest("مفتاح السطر غير صالح"));
    }

    [HttpDelete("{studentId:int}/plans/{planId:int}")]
    public async Task<IActionResult> ArchivePlan(
        int studentId,
        int planId,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var plan = await db.StudentPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.StudentId == studentId, cancellationToken);

        if (plan is null)
            return this.ToActionResult(GlobalResponse.NotFound("الخطة غير موجودة"));

        plan.IsArchived = true;
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم أرشفة الخطة"));
    }

    [HttpPost("{studentId:int}/log-status")]
    public async Task<IActionResult> LogPlanRowStatus(
        int studentId,
        [FromBody] LogPlanRowStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var status = PlanRowStatus.Normalize(request.Status);
        if (!PlanRowStatus.ValidStatuses.Contains(status))
            return this.ToActionResult(GlobalResponse.BadRequest("حالة غير صالحة"));

        StudentPlanMemorizing? memEntity = null;
        StudentPlanRevise? revEntity = null;

        if (request.RowKey.StartsWith("memorizing_")
            && int.TryParse(request.RowKey["memorizing_".Length..], out var memId))
        {
            memEntity = await db.StudentPlanMemorizings.FindAsync([memId], cancellationToken);
        }
        else if (request.RowKey.StartsWith("revise_")
                 && int.TryParse(request.RowKey["revise_".Length..], out var revId))
        {
            revEntity = await db.StudentPlanRevises.FindAsync([revId], cancellationToken);
        }
        else
        {
            return this.ToActionResult(GlobalResponse.BadRequest("مفتاح السطر غير صالح"));
        }

        if (memEntity is null && revEntity is null)
            return this.ToActionResult(GlobalResponse.NotFound("السجل غير موجود"));

        if (memEntity?.StudentId != studentId && revEntity?.StudentId != studentId)
            return this.ToActionResult(GlobalResponse.NotFound("السجل غير موجود"));

        var planId = memEntity?.PlanId ?? revEntity!.PlanId;
        var fromAyah = memEntity?.FromAyahNumber ?? revEntity!.FromAyahNumber;
        var originalToAyah = memEntity?.ToAyahNumber ?? revEntity!.ToAyahNumber;

        if (PlanRowStatus.IsPass(status) && request.ConfirmedToAyahNumber is int confirmedCheck
            && (confirmedCheck < fromAyah || confirmedCheck > originalToAyah))
        {
            return this.ToActionResult(GlobalResponse.BadRequest("إلى آية غير صالحة"));
        }

        var loggedAt = KuwaitTime.Now;
        db.StudentPlanItemLogs.Add(new StudentPlanItemLog
        {
            StudentId = studentId,
            PlanId = planId,
            RowKey = request.RowKey,
            Status = status,
            TeacherId = teacherId,
            LoggedAt = loggedAt
        });

        if (memEntity is not null)
        {
            var previousStatus = memEntity.Status;
            memEntity.Status = status;

            if (PlanRowStatus.IsPass(status) && !PlanRowStatus.IsPass(previousStatus))
            {
                memEntity.MemorizeDate = loggedAt.Date;

                var confirmed = await ApplyPassWithOptionalRemainderAsync(
                    studentId: studentId,
                    teacherId: teacherId,
                    circleId: circleId,
                    planId: planId,
                    isMemorizing: true,
                    surahId: memEntity.SurahId,
                    fromAyah: memEntity.FromAyahNumber,
                    originalToAyah: memEntity.ToAyahNumber,
                    rowPlanDate: memEntity.PlanDate,
                    memorizationLevel: memEntity.MemorizationLevel,
                    confirmedToAyah: request.ConfirmedToAyahNumber,
                    cancellationToken: cancellationToken);

                memEntity.ToAyahNumber = confirmed;
            }
        }
        else
        {
            var previousStatus = revEntity!.Status;
            revEntity.Status = status;

            if (PlanRowStatus.IsPass(status) && !PlanRowStatus.IsPass(previousStatus))
            {
                revEntity.ReviseDate = loggedAt.Date;

                var confirmed = await ApplyPassWithOptionalRemainderAsync(
                    studentId: studentId,
                    teacherId: teacherId,
                    circleId: circleId,
                    planId: planId,
                    isMemorizing: false,
                    surahId: revEntity.SurahId,
                    fromAyah: revEntity.FromAyahNumber,
                    originalToAyah: revEntity.ToAyahNumber,
                    rowPlanDate: revEntity.PlanDate,
                    memorizationLevel: revEntity.MemorizationLevel,
                    confirmedToAyah: request.ConfirmedToAyahNumber,
                    cancellationToken: cancellationToken);

                revEntity.ToAyahNumber = confirmed;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        var memStatuses = await db.StudentPlanMemorizings
            .Where(x => x.PlanId == planId).Select(x => x.Status).ToListAsync(cancellationToken);
        var revStatuses = await db.StudentPlanRevises
            .Where(x => x.PlanId == planId).Select(x => x.Status).ToListAsync(cancellationToken);
        var allStatuses = memStatuses.Concat(revStatuses).ToList();

        var plan = await db.StudentPlans.AsNoTracking().FirstAsync(p => p.Id == planId, cancellationToken);
        var circleDayNumbers = await GetWorkDayNumbersAsync(cancellationToken);
        var progress = StudentPlan2Helper.BuildProgress(
            allStatuses, plan.PlanFromDate, plan.PlanToDate, circleDayNumbers);

        var rowLabel = await GetRowLabelAsync(request.RowKey, planId, cancellationToken);
        var logRow = new AssessmentLogEntryDto
        {
            RowLabel = rowLabel,
            Status = status,
            StatusDisplay = PlanRowStatus.GetDisplayLabel(request.RowKey, status),
            TeacherName = User.FindFirstValue("name") ?? "",
            LoggedAtFormatted = loggedAt.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture)
        };

        PlanRowDto? nextRevise = null;
        var reviseStatuses = new[] { PlanRowStatus.Pass, PlanRowStatus.Fail, PlanRowStatus.Retake };
        if (request.RowKey.StartsWith("revise_") && reviseStatuses.Contains(status))
        {
            var nextRev = await db.StudentPlanRevises
                .Include(x => x.QuranSurah)
                .Where(x => x.StudentId == studentId
                    && x.PlanId == planId
                    && PlanRowStatus.PendingStatuses.Contains(x.Status))
                .OrderBy(x => x.PlanDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (nextRev is not null)
                nextRevise = StudentPlan2Helper.MapReviseRow(nextRev);
        }

        return this.ToActionResult(GlobalResponse.Ok(new LogPlanRowStatusResponseDto
        {
            Progress = progress,
            LogRow = logRow,
            NextReviseRecord = nextRevise
        }));
    }

    [HttpPost("{studentId:int}/next-memorize-date")]
    public async Task<IActionResult> SaveNextMemorizeDate(
        int studentId,
        [FromBody] SaveNextDateRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        if (!DateTime.TryParse(request.Date, out var selectedDate))
            return this.ToActionResult(GlobalResponse.BadRequest("تاريخ غير صالح"));

        await ApplyNextDateAsync(studentId, request.ItemKeys, selectedDate, isMemorize: true, cancellationToken);
        return this.ToActionResult(GlobalResponse.Ok(message: "تم حفظ التاريخ بنجاح"));
    }

    [HttpPost("{studentId:int}/next-revise-date")]
    public async Task<IActionResult> SaveNextReviseDate(
        int studentId,
        [FromBody] SaveNextDateRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await GetStudentForWriteAsync(studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        if (!DateTime.TryParse(request.Date, out var selectedDate))
            return this.ToActionResult(GlobalResponse.BadRequest("تاريخ غير صالح"));

        await ApplyNextDateAsync(studentId, request.ItemKeys, selectedDate, isMemorize: false, cancellationToken);
        return this.ToActionResult(GlobalResponse.Ok(message: "تم حفظ التاريخ بنجاح"));
    }

    private async Task<int> CreatePlanForStudentAsync(
        int studentId,
        SavePlanRowsRequestDto request,
        List<PlanRowInputDto> rows,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var plan = new StudentPlan
        {
            StudentId = studentId,
            Name = string.IsNullOrWhiteSpace(request.PlanName)
                ? "خطة جديدة " + planStart.ToString("yyyy-MM-dd")
                : request.PlanName.Trim(),
            PlanFromDate = planStart,
            PlanToDate = planEnd,
            CreatedAt = now
        };
        db.StudentPlans.Add(plan);
        await db.SaveChangesAsync(cancellationToken);

        await AddPlanRowsAsync(studentId, plan.Id, rows, planStart, planEnd, now, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return plan.Id;
    }

    private async Task AddPlanRowsAsync(
        int studentId,
        int planId,
        List<PlanRowInputDto> rows,
        DateTime planStart,
        DateTime planEnd,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var expanded = await StudentPlan2Helper.ExpandSurahRowsAsync(db, rows, cancellationToken);
        IReadOnlyList<int> workDays = expanded.Any(r => r.UseNextWorkDay)
            ? await GetWorkDayNumbersAsync(cancellationToken)
            : Array.Empty<int>();

        StudentPlan? planEntity = null;
        int? teacherId = null;
        int? circleId = null;

        foreach (var row in expanded)
        {
            var rowDate = planStart;
            var rowEnd = planEnd;

            if (row.UseNextWorkDay)
            {
                rowDate = AssignPlanHelper.GetNextWorkDay(planStart, workDays);
                rowEnd = rowDate;
            }
            else if (row.PlanDate.HasValue)
            {
                rowDate = row.PlanDate.Value.Date;
                rowEnd = rowDate;
            }

            if (row.UseNextWorkDay || row.PlanDate.HasValue)
            {
                planEntity ??= await db.StudentPlans.FirstAsync(p => p.Id == planId, cancellationToken);
                if (rowDate > planEntity.PlanToDate)
                    planEntity.PlanToDate = rowDate;
            }

            var status = string.IsNullOrWhiteSpace(row.Status)
                ? PlanRowStatus.Pending
                : PlanRowStatus.Normalize(row.Status);

            // Rows saved as already passed carry no later status update, so the completion
            // date has to be stamped here or the row keeps only its planned date.
            var completedOn = PlanRowStatus.IsPass(status) ? now.Date : (DateTime?)null;

            if (row.PlanType == "مراجعة")
            {
                db.StudentPlanRevises.Add(new StudentPlanRevise
                {
                    StudentId = studentId,
                    PlanId = planId,
                    MemorizationLevel = ManualPlanRowHelper.ResolveLevel(row.SurahName),
                    SurahId = string.IsNullOrWhiteSpace(row.SurahName)
                        ? row.SurahId
                        : ManualPlanRowHelper.PlaceholderSurahId,
                    FromAyahNumber = row.FromAyahNumber,
                    ToAyahNumber = row.ToAyahNumber,
                    PlanDate = rowDate,
                    PlanEndDate = rowEnd,
                    CreatedAt = now,
                    Status = status,
                    ReviseDate = completedOn
                });
            }
            else
            {
                db.StudentPlanMemorizings.Add(new StudentPlanMemorizing
                {
                    StudentId = studentId,
                    PlanId = planId,
                    MemorizationLevel = ManualPlanRowHelper.ResolveLevel(row.SurahName),
                    SurahId = string.IsNullOrWhiteSpace(row.SurahName)
                        ? row.SurahId
                        : ManualPlanRowHelper.PlaceholderSurahId,
                    FromAyahNumber = row.FromAyahNumber,
                    ToAyahNumber = row.ToAyahNumber,
                    PlanDate = rowDate,
                    PlanEndDate = rowEnd,
                    CreatedAt = now,
                    Status = status,
                    MemorizeDate = completedOn
                });
            }

            if (PlanRowStatus.IsPass(status))
            {
                if (teacherId is null || circleId is null)
                {
                    if (!TryGetTeacherContext(out var tid, out var cid))
                        continue;
                    teacherId = tid;
                    circleId = cid;
                }

                await AddMemorizingArchiveCardAsync(
                    studentId: studentId,
                    teacherId: teacherId.Value,
                    circleId: circleId.Value,
                    isMemorizing: row.PlanType != "مراجعة",
                    surahId: row.SurahId,
                    fromAyah: row.FromAyahNumber,
                    toAyah: row.ToAyahNumber,
                    manualSurahName: row.SurahName,
                    cancellationToken: cancellationToken);
            }
        }
    }

    private async Task ApplyNextDateAsync(
        int studentId,
        List<string> itemKeys,
        DateTime selectedDate,
        bool isMemorize,
        CancellationToken cancellationToken)
    {
        foreach (var itemKey in itemKeys)
        {
            if (itemKey.StartsWith("memorizing_") && int.TryParse(itemKey["memorizing_".Length..], out var memId))
            {
                var ent = await db.StudentPlanMemorizings
                    .FirstOrDefaultAsync(x => x.Id == memId && x.StudentId == studentId, cancellationToken);
                if (ent is null) continue;
                if (isMemorize)
                    ent.MemorizeDate = selectedDate;
                else
                    ent.ReviseDate = selectedDate;
            }
            else if (itemKey.StartsWith("revise_") && int.TryParse(itemKey["revise_".Length..], out var revId))
            {
                var ent = await db.StudentPlanRevises
                    .FirstOrDefaultAsync(x => x.Id == revId && x.StudentId == studentId, cancellationToken);
                if (ent is null) continue;
                if (isMemorize)
                    ent.MemorizeDate = selectedDate;
                else
                    ent.ReviseDate = selectedDate;
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> ResolvePlanIdFromRowKeyAsync(
        int studentId,
        string rowKey,
        CancellationToken cancellationToken)
    {
        if (rowKey.StartsWith("memorizing_") && int.TryParse(rowKey["memorizing_".Length..], out var memId))
        {
            return await db.StudentPlanMemorizings
                .Where(x => x.Id == memId && x.StudentId == studentId)
                .Select(x => x.PlanId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (rowKey.StartsWith("revise_") && int.TryParse(rowKey["revise_".Length..], out var revId))
        {
            return await db.StudentPlanRevises
                .Where(x => x.Id == revId && x.StudentId == studentId)
                .Select(x => x.PlanId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return 0;
    }

    private async Task<int> ApplyPassWithOptionalRemainderAsync(
        int studentId,
        int teacherId,
        int circleId,
        int planId,
        bool isMemorizing,
        int surahId,
        int fromAyah,
        int originalToAyah,
        DateTime rowPlanDate,
        string memorizationLevel,
        int? confirmedToAyah,
        CancellationToken cancellationToken)
    {
        var confirmed = confirmedToAyah ?? originalToAyah;

        if (confirmed < originalToAyah)
        {
            var workDays = await GetWorkDayNumbersAsync(cancellationToken);
            // Keep remainder on the same plan day so the student finishes the surah
            // before later surahs that were dumped on the same PlanDate.
            var remainderDate = rowPlanDate.Date;
            var bumpedDate = AssignPlanHelper.GetNextWorkDay(remainderDate, workDays);

            var plan = await db.StudentPlans.FirstAsync(p => p.Id == planId, cancellationToken);
            if (bumpedDate > plan.PlanToDate)
                plan.PlanToDate = bumpedDate;

            if (isMemorizing)
            {
                // Push other same-day (or earlier-dated) pending rows to the next work day
                // so "current" becomes the remainder of this surah, not التحريم/etc.
                var competing = await db.StudentPlanMemorizings
                    .Where(x => x.PlanId == planId
                        && x.StudentId == studentId
                        && x.SurahId != surahId
                        && x.PlanDate <= remainderDate)
                    .ToListAsync(cancellationToken);

                foreach (var row in competing.Where(x => !PlanRowStatus.IsPass(x.Status)))
                {
                    row.PlanDate = bumpedDate;
                    row.PlanEndDate = bumpedDate;
                }

                db.StudentPlanMemorizings.Add(new StudentPlanMemorizing
                {
                    StudentId = studentId,
                    PlanId = planId,
                    MemorizationLevel = memorizationLevel,
                    SurahId = surahId,
                    FromAyahNumber = confirmed + 1,
                    ToAyahNumber = originalToAyah,
                    PlanDate = remainderDate,
                    PlanEndDate = remainderDate,
                    CreatedAt = KuwaitTime.Now,
                    Status = PlanRowStatus.Pending
                });
            }
            else
            {
                var competing = await db.StudentPlanRevises
                    .Where(x => x.PlanId == planId
                        && x.StudentId == studentId
                        && x.SurahId != surahId
                        && x.PlanDate <= remainderDate)
                    .ToListAsync(cancellationToken);

                foreach (var row in competing.Where(x => !PlanRowStatus.IsPass(x.Status)))
                {
                    row.PlanDate = bumpedDate;
                    row.PlanEndDate = bumpedDate;
                }

                db.StudentPlanRevises.Add(new StudentPlanRevise
                {
                    StudentId = studentId,
                    PlanId = planId,
                    MemorizationLevel = memorizationLevel,
                    SurahId = surahId,
                    FromAyahNumber = confirmed + 1,
                    ToAyahNumber = originalToAyah,
                    PlanDate = remainderDate,
                    PlanEndDate = remainderDate,
                    CreatedAt = KuwaitTime.Now,
                    Status = PlanRowStatus.Pending
                });
            }
        }

        await AddMemorizingArchiveCardAsync(
            studentId: studentId,
            teacherId: teacherId,
            circleId: circleId,
            isMemorizing: isMemorizing,
            surahId: surahId,
            fromAyah: fromAyah,
            toAyah: confirmed,
            manualSurahName: ManualPlanRowHelper.IsManual(memorizationLevel)
                ? ManualPlanRowHelper.ExtractName(memorizationLevel)
                : null,
            cancellationToken: cancellationToken);

        return confirmed;
    }

    private async Task AddMemorizingArchiveCardAsync(
        int studentId,
        int teacherId,
        int circleId,
        bool isMemorizing,
        int surahId,
        int fromAyah,
        int toAyah,
        string? manualSurahName,
        CancellationToken cancellationToken)
    {
        var now = KuwaitTime.Now;
        var surahName = !string.IsNullOrWhiteSpace(manualSurahName)
            ? manualSurahName.Trim()
            : await db.QuranSurahs.AsNoTracking()
                .Where(s => s.Id == surahId)
                .Select(s => s.NameAr)
                .FirstOrDefaultAsync(cancellationToken) ?? "";

        db.StudentMemorizingCards.Add(new StudentMemorizingCard
        {
            CreatedAt = now,
            CircleId = circleId,
            DayName = now.ToString("dddd", CultureInfo.CurrentCulture),
            IsDone = isMemorizing ? "لا" : "نعم",
            TeacherId = teacherId,
            StudentId = studentId,
            TestFrom = fromAyah.ToString(CultureInfo.InvariantCulture),
            TestTo = toAyah.ToString(CultureInfo.InvariantCulture),
            SurahName = isMemorizing ? surahName : "",
            TheType = isMemorizing ? "حفظ" : "مراجعة",
            Notes = null,
            ParentNotes = null,
            IsSaveDone = isMemorizing ? "نعم" : "لا"
        });
    }

    private async Task<string> GetRowLabelAsync(
        string rowKey,
        int planId,
        CancellationToken cancellationToken)
    {
        if (rowKey.StartsWith("memorizing_") && int.TryParse(rowKey["memorizing_".Length..], out var memId))
        {
            var e = await db.StudentPlanMemorizings
                .Include(x => x.QuranSurah)
                .FirstOrDefaultAsync(x => x.Id == memId && x.PlanId == planId, cancellationToken);
            if (e is not null)
                return "حفظ: " + (e.QuranSurah?.NameAr ?? "—") + " " + e.FromAyahNumber + "-" + e.ToAyahNumber;
        }
        else if (rowKey.StartsWith("revise_") && int.TryParse(rowKey["revise_".Length..], out var revId))
        {
            var e = await db.StudentPlanRevises
                .Include(x => x.QuranSurah)
                .FirstOrDefaultAsync(x => x.Id == revId && x.PlanId == planId, cancellationToken);
            if (e is not null)
                return "مراجعة: " + (e.QuranSurah?.NameAr ?? "—") + " " + e.FromAyahNumber + "-" + e.ToAyahNumber;
        }

        return rowKey;
    }

    private async Task<List<int>> GetWorkDayNumbersAsync(CancellationToken cancellationToken) =>
        (await workDayService.GetWorkDayNumbersAsync(cancellationToken)).ToList();

    private Task<RegisterForm?> GetStudentInCircleAsync(
        int studentId,
        int circleId,
        CancellationToken cancellationToken) =>
        StudentCircleAccessHelper.GetStudentIfReadableAsync(db, studentId, circleId, cancellationToken);

    private Task<RegisterForm?> GetStudentForWriteAsync(
        int studentId,
        int circleId,
        CancellationToken cancellationToken) =>
        StudentCircleAccessHelper.GetStudentIfWritableAsync(db, studentId, circleId, cancellationToken, track: true);

    private int GetTeacherId()
    {
        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out var id) ? id : 0;
    }

    private bool TryGetTeacherContext(out int teacherId, out int circleId)
    {
        teacherId = GetTeacherId();
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return teacherId > 0 && int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }
}
