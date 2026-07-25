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
public class ParentQuestionsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByStudent(
        [FromQuery] int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var inCircle = await db.RegisterForms
            .AnyAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

        if (!inCircle)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود في الحلقة الحالية"));

        var questions = await db.ParentNotes
            .AsNoTracking()
            .Where(n => n.StudentId == studentId)
            .OrderByDescending(n => n.Id)
            .Select(n => new ParentQuestionDto
            {
                Id = n.Id,
                Notes = n.Notes,
                CreatedDate = n.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                TeacherReply = n.TeacherReply,
                IsRead = n.IsRead
            })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(questions));
    }

    [HttpPut("{questionId:int}/reply")]
    public async Task<IActionResult> UpdateTeacherReply(
        int questionId,
        [FromBody] UpdateTeacherReplyRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out _, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var question = await FindQuestionInCircleAsync(questionId, circleId, cancellationToken);
        if (question is null)
            return this.ToActionResult(GlobalResponse.NotFound("السؤال غير موجود"));

        question.TeacherReply = request.Reply ?? "";
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم حفظ الرد بنجاح"));
    }

    [HttpPost("{questionId:int}/mark-read")]
    public async Task<IActionResult> MarkAsRead(
        int questionId,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var question = await FindQuestionInCircleAsync(questionId, circleId, cancellationToken);
        if (question is null)
            return this.ToActionResult(GlobalResponse.NotFound("السؤال غير موجود"));

        question.IsRead = true;
        question.ReadDate = KuwaitTime.Now;
        question.ReadByTeacherId = teacherId;
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم تحديد السؤال كمقروء"));
    }

    private async Task<Entities.ParentNote?> FindQuestionInCircleAsync(
        int questionId,
        int circleId,
        CancellationToken cancellationToken) =>
        await db.ParentNotes
            .Include(n => n.RegisterForm)
            .FirstOrDefaultAsync(
                n => n.Id == questionId && n.RegisterForm != null && n.RegisterForm.QuranCircleId == circleId,
                cancellationToken);

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
