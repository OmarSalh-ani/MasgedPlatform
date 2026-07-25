using System.Globalization;
using System.Security.Claims;
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
public class ReviseController(AppDbContext db) : ControllerBase
{
    private const string TypeMemorization = "حفظ";
    private const string TypeRevision = "مراجعة";

    [HttpGet("{studentId:int}")]
    public async Task<IActionResult> GetPage(int studentId, CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var student = await StudentCircleAccessHelper.GetStudentIfReadableAsync(
            db, studentId, circleId, cancellationToken);
        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var surahs = await db.QuranSurahs
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new IdNameDto { Id = x.Id, Name = x.NameAr })
            .ToListAsync(cancellationToken);

        var reviews = await LoadReviewsAsync(studentId, circleId, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new RevisePageDto
        {
            StudentId = student.Id,
            StudentName = student.StudentName,
            Surahs = surahs,
            Reviews = reviews
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

    [HttpGet("{studentId:int}/reviews/{reviewId:int}")]
    public async Task<IActionResult> GetReviewForEdit(
        int studentId,
        int reviewId,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (await StudentCircleAccessHelper.GetStudentIfReadableAsync(
                db, studentId, circleId, cancellationToken) is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var review = await db.StudentMemorizingCards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == reviewId && x.StudentId == studentId, cancellationToken);

        if (review is null)
            return this.ToActionResult(GlobalResponse.NotFound("السجل غير موجود"));

        if (!await StudentCircleAccessHelper.CanReadRecordAsync(
                db, studentId, circleId, review.CircleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.NotFound("السجل غير موجود"));

        var dto = await MapReviewAsync(review, circleId, cancellationToken);
        return this.ToActionResult(GlobalResponse.Ok(dto));
    }

    [HttpPost("{studentId:int}")]
    public async Task<IActionResult> Create(
        int studentId,
        [FromBody] CreateReviseRequestDto request,
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

        var generalNotes = request.ParentNotes ?? "";
        var createdIds = new List<int>();
        var now = KuwaitTime.Now;
        var dayName = now.ToString("dddd", CultureInfo.CurrentCulture);

        if (request.Memorization is { } mem
            && !string.IsNullOrEmpty(mem.TestFrom)
            && !string.IsNullOrEmpty(mem.TestTo))
        {
            var surahName = await GetSurahNameAsync(mem.SurahId, cancellationToken);
            var card = new StudentMemorizingCard
            {
                CreatedAt = now,
                CircleId = circleId,
                DayName = dayName,
                IsDone = "لا",
                TeacherId = teacherId,
                StudentId = studentId,
                TestFrom = mem.TestFrom,
                TestTo = mem.TestTo,
                SurahName = surahName,
                TheType = TypeMemorization,
                Notes = mem.NextMemorization,
                ParentNotes = generalNotes,
                IsSaveDone = mem.IsSaveCompleted ? "نعم" : "لا"
            };
            db.StudentMemorizingCards.Add(card);
            await db.SaveChangesAsync(cancellationToken);
            createdIds.Add(card.Id);

            await QueueReviseWhatsAppAsync(
                student,
                TypeMemorization,
                surahName,
                mem.TestFrom,
                mem.TestTo,
                generalNotes,
                cancellationToken);
        }

        if (request.Revision is { } rev
            && !string.IsNullOrEmpty(rev.TestFrom)
            && !string.IsNullOrEmpty(rev.TestTo))
        {
            var card = new StudentMemorizingCard
            {
                CreatedAt = now,
                CircleId = circleId,
                DayName = dayName,
                IsDone = rev.IsRevisionCompleted ? "نعم" : "لا",
                TeacherId = teacherId,
                StudentId = studentId,
                TestFrom = rev.TestFrom,
                TestTo = rev.TestTo,
                SurahName = "",
                TheType = TypeRevision,
                Notes = rev.NextRevise,
                ParentNotes = generalNotes,
                IsSaveDone = "لا"
            };
            db.StudentMemorizingCards.Add(card);
            await db.SaveChangesAsync(cancellationToken);
            createdIds.Add(card.Id);

            await QueueReviseWhatsAppAsync(
                student,
                TypeRevision,
                "",
                rev.TestFrom,
                rev.TestTo,
                generalNotes,
                cancellationToken);
        }

        if (createdIds.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إدخال بيانات الحفظ أو المراجعة"));

        var reviews = await LoadReviewsAsync(studentId, circleId, cancellationToken);
        return this.ToActionResult(GlobalResponse.Created(new { createdIds, reviews }));
    }

    [HttpPut("{studentId:int}/reviews/{reviewId:int}")]
    public async Task<IActionResult> Update(
        int studentId,
        int reviewId,
        [FromBody] UpdateReviseRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!await StudentCircleAccessHelper.CanWriteStudentAsync(db, studentId, circleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var existing = await db.StudentMemorizingCards
            .FirstOrDefaultAsync(x => x.Id == reviewId && x.StudentId == studentId, cancellationToken);

        if (existing is null)
            return this.ToActionResult(GlobalResponse.NotFound("السجل غير موجود"));

        if (!await StudentCircleAccessHelper.CanWriteRecordAsync(
                db, studentId, circleId, existing.CircleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.Forbidden("لا يمكن تعديل سجل من حلقة سابقة"));

        existing.ParentNotes = request.ParentNotes ?? "";

        if (existing.TheType == TypeMemorization && request.Memorization is { } mem)
        {
            existing.TestFrom = mem.TestFrom;
            existing.TestTo = mem.TestTo;
            existing.SurahName = await GetSurahNameAsync(mem.SurahId, cancellationToken);
            existing.Notes = mem.NextMemorization;
            existing.IsSaveDone = mem.IsSaveCompleted ? "نعم" : "لا";
        }
        else if (existing.TheType == TypeRevision && request.Revision is { } rev)
        {
            existing.TestFrom = rev.TestFrom;
            existing.TestTo = rev.TestTo;
            existing.SurahName = "";
            existing.IsDone = rev.IsRevisionCompleted ? "نعم" : "لا";
            existing.Notes = rev.NextRevise;
        }

        await db.SaveChangesAsync(cancellationToken);

        var dto = await MapReviewAsync(existing, circleId, cancellationToken);
        return this.ToActionResult(GlobalResponse.Ok(dto, "تم التعديل بنجاح"));
    }

    [HttpDelete("{studentId:int}/reviews/{reviewId:int}")]
    public async Task<IActionResult> Delete(
        int studentId,
        int reviewId,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!await StudentCircleAccessHelper.CanWriteStudentAsync(db, studentId, circleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var record = await db.StudentMemorizingCards
            .FirstOrDefaultAsync(x => x.Id == reviewId && x.StudentId == studentId, cancellationToken);

        if (record is null)
            return this.ToActionResult(GlobalResponse.NotFound("السجل غير موجود"));

        if (!await StudentCircleAccessHelper.CanWriteRecordAsync(
                db, studentId, circleId, record.CircleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.Forbidden("لا يمكن حذف سجل من حلقة سابقة"));

        db.StudentMemorizingCards.Remove(record);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم الحذف بنجاح"));
    }

    private async Task<List<StudentReviewDto>> LoadReviewsAsync(
        int studentId,
        int circleId,
        CancellationToken cancellationToken)
    {
        var isCurrentMember = await StudentCircleAccessHelper.IsCurrentMemberAsync(
            db, studentId, circleId, cancellationToken);

        var reviews = await db.StudentMemorizingCards
            .AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        var result = new List<StudentReviewDto>();
        foreach (var review in reviews)
        {
            if (!isCurrentMember && review.CircleId != circleId)
                continue;

            result.Add(await MapReviewAsync(review, circleId, cancellationToken));
        }

        return result;
    }

    private async Task<StudentReviewDto> MapReviewAsync(
        StudentMemorizingCard review,
        int teacherCircleId,
        CancellationToken cancellationToken)
    {
        int? surahId = null;
        if (review.TheType == TypeMemorization && !string.IsNullOrEmpty(review.SurahName))
        {
            surahId = await db.QuranSurahs
                .AsNoTracking()
                .Where(s => s.NameAr == review.SurahName)
                .Select(s => (int?)s.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var circleName = await db.QuranCircles
            .AsNoTracking()
            .Where(c => c.Id == review.CircleId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? "";

        var canEdit = await StudentCircleAccessHelper.CanWriteRecordAsync(
            db, review.StudentId, teacherCircleId, review.CircleId, cancellationToken);

        return new StudentReviewDto
        {
            Id = review.Id,
            CircleId = review.CircleId,
            CircleName = circleName,
            CanEdit = canEdit,
            ReviewType = review.TheType,
            CreatedAt = review.CreatedAt,
            TestFrom = review.TestFrom,
            TestTo = review.TestTo,
            SurahName = review.SurahName,
            IsDone = review.IsDone,
            DayName = review.DayName,
            Notes = review.Notes,
            ParentNotes = review.ParentNotes,
            IsSaveDone = review.IsSaveDone,
            SurahId = surahId
        };
    }

    private async Task<string> GetSurahNameAsync(int surahId, CancellationToken cancellationToken)
    {
        var name = await db.QuranSurahs
            .AsNoTracking()
            .Where(s => s.Id == surahId)
            .Select(s => s.NameAr)
            .FirstOrDefaultAsync(cancellationToken);

        return name ?? "";
    }

    private async Task QueueReviseWhatsAppAsync(
        RegisterForm student,
        string reviseType,
        string surahName,
        string testFrom,
        string testTo,
        string notes,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(student.FatherPhone))
            return;

        var message = await WhatsappMessageHelper.GetReviseFormattedMessageAsync(
            db,
            student,
            student.QuranCircle?.Name,
            student.QuranCircle?.Teacher?.Name,
            reviseType,
            surahName,
            testFrom,
            testTo,
            notes,
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
