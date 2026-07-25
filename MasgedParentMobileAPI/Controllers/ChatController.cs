using System.Security.Claims;

using MasgedParentMobileAPI.DTOs;

using MasgedParentMobileAPI.Services;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;



namespace MasgedParentMobileAPI.Controllers;



[Authorize]

[ApiController]

[Route("api/chat")]

public sealed class ChatController(ChatService chatService, IChatRealtimePublisher publisher)

    : ControllerBase

{

    [HttpGet("conversations")]

    public async Task<ActionResult<List<ChatConversationDto>>> GetConversations(CancellationToken cancellationToken)

    {

        cancellationToken.ThrowIfCancellationRequested();



        var fatherPhone = User.FindFirstValue("fatherPhone");

        if (string.IsNullOrEmpty(fatherPhone))

            return Unauthorized();



        return Ok(await chatService.GetConversationsForParentAsync(fatherPhone));

    }



    [HttpGet("teachers/{teacherId:int}/students/{studentId:int}/messages")]

    public async Task<ActionResult<IReadOnlyList<ChatMessageDto>>> GetMessages(

        int teacherId,

        int studentId,

        [FromQuery] int? beforeId,

        [FromQuery] int take = 50,

        CancellationToken cancellationToken = default)

    {

        cancellationToken.ThrowIfCancellationRequested();



        var fatherPhone = User.FindFirstValue("fatherPhone");

        if (string.IsNullOrEmpty(fatherPhone))

            return Unauthorized();



        try

        {

            return Ok(await chatService.GetMessagesForParentAsync(

                fatherPhone,

                teacherId,

                studentId,

                beforeId,

                take));

        }

        catch (UnauthorizedAccessException)

        {

            return Forbid();

        }

    }



    [HttpPost("teachers/{teacherId:int}/students/{studentId:int}/messages")]

    public async Task<ActionResult<ChatMessageDto>> SendMessage(

        int teacherId,

        int studentId,

        [FromBody] SendChatMessageRequest request,

        CancellationToken cancellationToken)

    {

        var fatherPhone = User.FindFirstValue("fatherPhone");

        if (string.IsNullOrEmpty(fatherPhone))

            return Unauthorized();



        try

        {

            var msg = await chatService.SendParentMessageAsync(

                fatherPhone,

                teacherId,

                studentId,

                request,

                publisher,

                cancellationToken);

            return Ok(msg);

        }

        catch (UnauthorizedAccessException)

        {

            return Forbid();

        }

        catch (ArgumentException)

        {

            return BadRequest(new { message = "الرسالة غير صالحة" });

        }

    }



    [HttpPost("teachers/{teacherId:int}/students/{studentId:int}/mark-read")]

    public async Task<IActionResult> MarkRead(

        int teacherId,

        int studentId,

        [FromBody] MarkChatReadRequest? body = null)

    {

        var fatherPhone = User.FindFirstValue("fatherPhone");

        if (string.IsNullOrEmpty(fatherPhone))

            return Unauthorized();



        try

        {

            await chatService.MarkReadForParentAsync(

                PhoneNormalizer.ToCanonical(fatherPhone),

                teacherId,

                studentId,

                body?.UpToMessageId);



            await publisher.PublishMessagesRead(

                studentId,

                teacherId,

                "parent",

                CancellationToken.None);



            return NoContent();

        }

        catch (UnauthorizedAccessException)

        {

            return Forbid();

        }

    }

}
