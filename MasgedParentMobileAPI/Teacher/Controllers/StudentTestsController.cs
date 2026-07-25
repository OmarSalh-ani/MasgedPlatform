using System.Globalization;
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
[Route("api/students/{studentId:int}/tests")]
public class StudentTestsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTests(int studentId, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var student = await StudentCircleAccessHelper.GetStudentIfReadableAsync(
            db, studentId, circleId, cancellationToken);
        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var isCurrentMember = student.QuranCircleId == circleId;

        var tests = await db.TestHeads
            .AsNoTracking()
            .Where(t => t.StudentId == studentId)
            .OrderByDescending(t => t.TestDate)
            .Select(t => new
            {
                t.Id,
                t.TestDate,
                t.SurahName,
                t.HezbNumber,
                t.TestFrom,
                t.TestTo,
                t.FinalResult,
                t.Notes,
                t.CircleId,
                CircleName = t.QuranCircle != null ? t.QuranCircle.Name : ""
            })
            .ToListAsync(cancellationToken);

        var items = tests
            .Where(t => isCurrentMember || t.CircleId == circleId)
            .Select(t => new StudentTestListItemDto
            {
                TestId = t.Id,
                TestName = t.TestDate.ToString("yyyy/MM/dd HH:mm"),
                SurahName = t.SurahName ?? "",
                HezbNumber = t.HezbNumber ?? "",
                From = t.TestFrom ?? "",
                To = t.TestTo ?? "",
                TestDegree = t.FinalResult.ToString("N0", CultureInfo.InvariantCulture),
                Notes = t.Notes,
                CircleId = t.CircleId,
                CircleName = t.CircleName,
                CanEdit = isCurrentMember && t.CircleId == circleId
            })
            .ToList();

        return this.ToActionResult(GlobalResponse.Ok(new StudentTestsPageDto
        {
            StudentId = student.Id,
            StudentName = student.StudentName ?? "",
            Tests = items
        }));
    }

    [HttpGet("{testId:int}")]
    public async Task<IActionResult> GetTest(int studentId, int testId, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var student = await StudentCircleAccessHelper.GetStudentIfReadableAsync(
            db, studentId, circleId, cancellationToken);
        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var testHead = await db.TestHeads
            .AsNoTracking()
            .Include(t => t.QuranCircle)
            .FirstOrDefaultAsync(t => t.Id == testId && t.StudentId == studentId, cancellationToken);

        if (testHead is null)
            return this.ToActionResult(GlobalResponse.NotFound("الاختبار غير موجود"));

        if (!await StudentCircleAccessHelper.CanReadRecordAsync(
                db, studentId, circleId, testHead.CircleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.NotFound("الاختبار غير موجود"));

        var questions = await db.TestBodies
            .AsNoTracking()
            .Where(tb => tb.TestHeadId == testId)
            .OrderBy(tb => tb.QuestionOrder)
            .Select(tb => new StudentTestQuestionDto
            {
                QuestionName = tb.QuestionName,
                TestDegree = tb.TestDegree,
                QuestionOrder = tb.QuestionOrder,
                CreatedAt = tb.CreatedAt.ToString("yyyy/MM/dd HH:mm")
            })
            .ToListAsync(cancellationToken);

        var canEdit = await StudentCircleAccessHelper.CanWriteRecordAsync(
            db, studentId, circleId, testHead.CircleId, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new StudentTestDetailDto
        {
            TestId = testHead.Id,
            StudentId = testHead.StudentId,
            CircleId = testHead.CircleId,
            CircleName = testHead.QuranCircle?.Name ?? "",
            CanEdit = canEdit,
            TestDate = testHead.TestDate.ToString("yyyy-MM-ddTHH:mm"),
            FinalResult = testHead.FinalResult.ToString("N0", CultureInfo.InvariantCulture),
            SurahName = testHead.SurahName ?? "",
            HezbNumber = testHead.HezbNumber ?? "",
            FromSurah = testHead.TestFrom ?? "",
            ToSurah = testHead.TestTo ?? "",
            Notes = testHead.Notes,
            MemorizationScore = testHead.MemorizationScore,
            TajweedScore = testHead.TajweedScore,
            RevisionScore = testHead.RevisionScore,
            TotalScore = testHead.TotalScore,
            Grade = testHead.Grade,
            Questions = questions
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTest(
        int studentId,
        [FromBody] SaveStudentTestRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var student = await db.RegisterForms
            .Include(s => s.QuranCircle)!
            .ThenInclude(c => c!.Teacher)
            .FirstOrDefaultAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var testDate = request.TestDate ?? KuwaitTime.Now;
        var totalScore = (decimal?)request.TotalScore;
        var grade = !string.IsNullOrWhiteSpace(request.Grade)
            ? request.Grade.Trim()
            : StudentTestsHelper.CalculateGrade(totalScore);

        var testHead = new TestHead
        {
            StudentId = studentId,
            CircleId = circleId,
            TeacherId = teacherId,
            SurahName = request.SurahName ?? "",
            HezbNumber = request.HezbNumber ?? "",
            TestFrom = "",
            TestTo = "",
            TestDate = testDate,
            FinalResult = totalScore ?? 0,
            MemorizationScore = request.MemorizationScore,
            TajweedScore = request.TajweedScore,
            RevisionScore = request.RevisionScore,
            TotalScore = totalScore,
            Grade = grade,
            Notes = request.Notes,
            CreatedAt = KuwaitTime.Now
        };

        db.TestHeads.Add(testHead);
        await db.SaveChangesAsync(cancellationToken);

        await QueueTestWhatsAppAsync(student, testHead, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(
            new { testId = testHead.Id },
            "تم حفظ الاختبار بنجاح"));
    }

    [HttpPut("{testId:int}")]
    public async Task<IActionResult> UpdateTest(
        int studentId,
        int testId,
        [FromBody] SaveStudentTestRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var student = await db.RegisterForms
            .Include(s => s.QuranCircle)!
            .ThenInclude(c => c!.Teacher)
            .FirstOrDefaultAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var testHead = await db.TestHeads
            .FirstOrDefaultAsync(t => t.Id == testId && t.StudentId == studentId, cancellationToken);

        if (testHead is null)
            return this.ToActionResult(GlobalResponse.NotFound("الاختبار غير موجود"));

        if (!await StudentCircleAccessHelper.CanWriteRecordAsync(
                db, studentId, circleId, testHead.CircleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.Forbidden("لا يمكن تعديل اختبار من حلقة سابقة"));

        var validRows = StudentTestsHelper.GetValidRows(request.TestRows);
        var firstRow = validRows.FirstOrDefault();
        var testDate = request.TestDate ?? testHead.TestDate;
        var totalScore = (decimal?)request.TotalScore;
        var grade = !string.IsNullOrWhiteSpace(request.Grade)
            ? request.Grade.Trim()
            : StudentTestsHelper.CalculateGrade(totalScore);

        testHead.SurahName = firstRow?.SurahName ?? request.SurahName ?? "";
        testHead.HezbNumber = firstRow?.HezbNumber ?? request.HezbNumber ?? "";
        testHead.TestFrom = firstRow?.FromSurah ?? "";
        testHead.TestTo = firstRow?.ToSurah ?? "";
        testHead.TestDate = testDate;
        testHead.FinalResult = StudentTestsHelper.ComputeFinalResultFromRows(validRows);
        testHead.MemorizationScore = request.MemorizationScore;
        testHead.TajweedScore = request.TajweedScore;
        testHead.RevisionScore = request.RevisionScore;
        testHead.TotalScore = totalScore;
        testHead.Grade = grade;
        testHead.Notes = request.Notes;

        var existingBodies = await db.TestBodies
            .Where(x => x.TestHeadId == testId)
            .ToListAsync(cancellationToken);
        db.TestBodies.RemoveRange(existingBodies);

        foreach (var row in validRows)
        {
            if (!int.TryParse(row.Degree, out var degree))
                continue;

            db.TestBodies.Add(new TestBody
            {
                TestHeadId = testId,
                QuestionName = row.Question,
                QuestionOrder = row.RowNumber,
                TestDegree = degree,
                CreatedAt = KuwaitTime.Now
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        await QueueTestWhatsAppAsync(student, testHead, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(
            new { testId = testHead.Id, questionCount = validRows.Count },
            $"تم تحديث الاختبار بنجاح مع {validRows.Count} سؤال"));
    }

    [HttpDelete("{testId:int}")]
    public async Task<IActionResult> DeleteTest(int studentId, int testId, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!await StudentCircleAccessHelper.CanWriteStudentAsync(db, studentId, circleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var testHead = await db.TestHeads
            .FirstOrDefaultAsync(t => t.Id == testId && t.StudentId == studentId, cancellationToken);

        if (testHead is null)
            return this.ToActionResult(GlobalResponse.NotFound("الاختبار غير موجود"));

        if (!await StudentCircleAccessHelper.CanWriteRecordAsync(
                db, studentId, circleId, testHead.CircleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.Forbidden("لا يمكن حذف اختبار من حلقة سابقة"));

        var bodies = await db.TestBodies.Where(x => x.TestHeadId == testId).ToListAsync(cancellationToken);
        db.TestBodies.RemoveRange(bodies);
        db.TestHeads.Remove(testHead);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم حذف الاختبار بنجاح"));
    }

    private async Task QueueTestWhatsAppAsync(
        RegisterForm student,
        TestHead testHead,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(student.FatherPhone))
            return;

        var message = await WhatsappMessageHelper.GetTestFormattedMessageAsync(
            db,
            student,
            student.QuranCircle?.Name,
            student.QuranCircle?.Teacher?.Name,
            testHead.SurahName,
            testHead.TestFrom ?? "",
            testHead.TestTo ?? "",
            testHead.Notes,
            testHead.TestDate,
            testHead.HezbNumber,
            testHead.MemorizationScore,
            testHead.TajweedScore,
            testHead.RevisionScore,
            testHead.TotalScore,
            testHead.Grade,
            testHead.FinalResult,
            cancellationToken);

        if (string.IsNullOrEmpty(message))
            return;

        var isGirl = bool.TryParse(User.FindFirstValue("isGirlTeacher"), out var girl) && girl;

        db.WhatsappTempTables.Add(new WhatsappTempTable
        {
            mobile = student.FatherPhone,
            IsGirl = isGirl ? 1 : 0,
            message = message
        });

        await db.SaveChangesAsync(cancellationToken);
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
