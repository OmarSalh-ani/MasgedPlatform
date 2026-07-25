using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using MasgedTeacherMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ChatController(
    ChatService chat,
    ChatBroadcastClient broadcast,
    ILogger<ChatController> logger) : ControllerBase
{
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations(CancellationToken cancellationToken)
    {
        if (!ChatService.TryParseTeacherContext(User, out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        try
        {
            var rows = await chat.GetTeacherConversationsAsync(teacherId, circleId).ConfigureAwait(false);
            return this.ToActionResult(GlobalResponse.Ok(rows));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetConversations failed for teacher {TeacherId}, circle {CircleId}", teacherId, circleId);
            return this.ToActionResult(GlobalResponse.InternalServerError("تعذر تحميل المحادثات"));
        }
    }

    [HttpGet("students/{studentId:int}/messages")]
    public async Task<IActionResult> GetMessages(
        int studentId,
        [FromQuery] int teacherId,
        [FromQuery] int? beforeId,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var list = await chat.GetMessagesForTeacherAsync(User, studentId, teacherId, beforeId, take)
                .ConfigureAwait(false);

            return this.ToActionResult(GlobalResponse.Ok(list));
        }
        catch (UnauthorizedAccessException)
        {
            return this.ToActionResult(GlobalResponse.Unauthorized());
        }
    }

    [HttpPost("students/{studentId:int}/messages")]
    public async Task<IActionResult> SendMessage(
        int studentId,
        [FromQuery] int teacherId,
        [FromBody] SendChatMessageRequestDto body,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await chat.SendTeacherMessageAsync(User, studentId, teacherId, body, broadcast, cancellationToken)
                .ConfigureAwait(false);

            return this.ToActionResult(GlobalResponse.Ok(dto));
        }
        catch (UnauthorizedAccessException)
        {
            return this.ToActionResult(GlobalResponse.Unauthorized());
        }
        catch (ArgumentException)
        {
            return this.ToActionResult(GlobalResponse.BadRequest("الرسالة غير صالحة"));
        }
    }

    [HttpPost("students/{studentId:int}/mark-read")]
    public async Task<IActionResult> MarkRead(int studentId, [FromQuery] int teacherId,
        [FromBody] MarkChatReadRequestDto? body = null)
    {
        try
        {
            await chat.MarkReadAsTeacherAsync(User, studentId, teacherId, body).ConfigureAwait(false);

            return this.ToActionResult(
                GlobalResponse.Ok(new { ok = true }, "تم تحديد الرسائل كمقروءة"));
        }
        catch (UnauthorizedAccessException)
        {
            return this.ToActionResult(GlobalResponse.Unauthorized());
        }
    }
}
