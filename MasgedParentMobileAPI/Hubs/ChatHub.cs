using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MasgedParentMobileAPI.Hubs;

[Authorize(AuthenticationSchemes = "Bearer,TeacherJwt")]
public sealed class ChatHub(ChatService chatService, IChatRealtimePublisher publisher)
    : Hub
{
    public async Task JoinConversation(int studentId, int teacherId)
    {
        if (await chatService.CanUserJoinConversationAsync(Context.User!, studentId, teacherId) == false)
            throw new HubException("غير مصرح بالانضمام لهذه المحادثة.");

        await Groups.AddToGroupAsync(Context.ConnectionId,
            ChatGroupNaming.For(studentId, teacherId));
    }

    public async Task LeaveConversation(int studentId, int teacherId) =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId,
            ChatGroupNaming.For(studentId, teacherId));

    public async Task<ChatMessageDto> SendConversationMessage(SendChatMessageRequest dto,
        int studentId,
        int teacherId)
    {
        if (dto?.MessageText is null || string.IsNullOrWhiteSpace(dto.MessageText))
            throw new HubException("الرسالة فارغة.");

        try
        {
            if (IsTeacherUser(Context.User))
            {
                var teacherSub =
                    Context.User!.FindFirstValue("id") ?? Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(teacherSub, out var tid) || tid != teacherId)
                    throw new HubException("معرف المعلم غير صالح.");

                if (dto.StudentId is int sid && sid != studentId)
                    throw new HubException("معرف الطالب غير متطابق.");

                var teacherReq = new SendChatMessageRequest
                {
                    MessageText = dto.MessageText.Trim(),
                    StudentId = studentId,
                };

                var saved = await chatService.SendTeacherMessageAsync(
                    Context.User!,
                    teacherId,
                    studentId,
                    teacherReq,
                    publisher,
                    Context.ConnectionAborted);

                if (saved is null)
                    throw new HubException("غير مصرح بإرسال الرسالة.");

                return saved;
            }

            var fatherPhone = Context.User!.FindFirstValue("fatherPhone");
            if (string.IsNullOrEmpty(fatherPhone))
                throw new HubException("تعذر التحقق من ولي الأمر.");

            var jwtCanon = PhoneNormalizer.ToCanonical(fatherPhone);

            if (dto.StudentId is int bodySid && bodySid != studentId)
                throw new HubException("معرف الطالب غير متطابق.");

            var parentReq = new SendChatMessageRequest
            {
                MessageText = dto.MessageText.Trim(),
                StudentId = studentId,
            };

            return await chatService.SendParentMessageAsync(
                jwtCanon,
                teacherId,
                studentId,
                parentReq,
                publisher,
                Context.ConnectionAborted) ??
                   throw new HubException("تعذر حفظ الرسالة.");
        }
        catch (UnauthorizedAccessException)
        {
            throw new HubException("غير مصرح بإرسال الرسالة.");
        }
        catch (ArgumentException)
        {
            throw new HubException("نص الرسالة غير صالح.");
        }
    }

    public async Task MarkConversationRead(int studentId, int teacherId)
    {
        var readSide = IsTeacherUser(Context.User) ? "teacher" : "parent";

        if (IsTeacherUser(Context.User))
        {
            await chatService.MarkReadForTeacherAsync(Context.User!, teacherId, studentId);
        }
        else
        {
            var fp = Context.User!.FindFirstValue("fatherPhone");
            if (string.IsNullOrEmpty(fp))
                throw new HubException("لم يتم التعرف على ولي الأمر.");

            await chatService.MarkReadForParentAsync(
                PhoneNormalizer.ToCanonical(fp),
                teacherId,
                studentId);
        }

        await publisher.PublishMessagesRead(studentId, teacherId, readSide, Context.ConnectionAborted);
    }

    private static bool IsTeacherUser(ClaimsPrincipal? user) =>
        user?.FindFirstValue("fatherPhone") is null or "" &&
        user?.FindFirstValue("circleId") is { Length: > 0 };
}
