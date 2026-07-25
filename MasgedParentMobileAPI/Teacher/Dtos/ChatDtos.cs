namespace MasgedTeacherMobileAPI.Dtos;

public sealed class ChatMessageDto
{
    public int Id { get; set; }
    public string ParentPhone { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public byte SenderType { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public int? StudentId { get; set; }
    public DateTimeOffset SentAt { get; set; }
}

public sealed class ChatConversationDto
{
    public string ParentPhone { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public string? StudentName { get; set; }

    public int TeacherId { get; set; }

    public string? ParentDisplayName { get; set; }

    public string? LastMessagePreview { get; set; }

    public DateTimeOffset? LastMessageAt { get; set; }

    public int UnreadCount { get; set; }
}

public sealed class SendChatMessageRequestDto
{
    public string MessageText { get; set; } = string.Empty;

    public int? StudentId { get; set; }
}

public sealed class MarkChatReadRequestDto
{
    public int? UpToMessageId { get; set; }
}

public static class ChatSenderTypes
{
    public const byte Parent = 0;
    public const byte Teacher = 1;
}
