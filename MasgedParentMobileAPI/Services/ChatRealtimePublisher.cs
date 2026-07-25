using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MasgedParentMobileAPI.Services;

public interface IChatRealtimePublisher
{
    Task PublishReceiveMessage(ChatMessageDto message, CancellationToken cancellationToken = default);

    Task PublishMessagesRead(int studentId, int teacherId, string readBySide,
        CancellationToken cancellationToken = default);
}

internal sealed class ChatRealtimePublisher(IHubContext<ChatHub> hubContext) : IChatRealtimePublisher
{
    public Task PublishReceiveMessage(ChatMessageDto message, CancellationToken cancellationToken = default)
    {
        if (message.StudentId is null || message.StudentId <= 0)
            return Task.CompletedTask;

        var group = ChatGroupNaming.For(message.StudentId.Value, message.TeacherId);
        return hubContext.Clients.Group(group).SendAsync("ReceiveMessage", message, cancellationToken);
    }

    public Task PublishMessagesRead(int studentId, int teacherId, string readBySide,
        CancellationToken cancellationToken = default)
    {
        var group = ChatGroupNaming.For(studentId, teacherId);
        return hubContext.Clients.Group(group)
            .SendAsync("MessagesRead", new { studentId, teacherId, readBySide }, cancellationToken);
    }
}
