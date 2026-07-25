namespace MasgedParentMobileAPI.DTOs;

public sealed class ChatMessageDto
{
    public int Id { get; init; }
    public string ParentPhone { get; init; } = string.Empty;
    public int TeacherId { get; init; }
    public byte SenderType { get; init; }
    public string MessageText { get; init; } = string.Empty;
    public int? StudentId { get; init; }
    public DateTimeOffset SentAt { get; init; }
}

public sealed class ChatConversationDto
{
    /// <summary>Canonical parent phone key (8-digit) for the student’s guardian.</summary>
    public string ParentPhone { get; init; } = string.Empty;

    public int StudentId { get; init; }

    public string? StudentName { get; init; }

    public int TeacherId { get; init; }

    public string? TeacherName { get; init; }

    public string? LastMessagePreview { get; init; }

    public DateTimeOffset? LastMessageAt { get; init; }

    public int UnreadCount { get; init; }
}

public sealed class SendChatMessageRequest
{
    public string MessageText { get; set; } = string.Empty;

    /// <summary>Optional; must match route studentId when both are present.</summary>
    public int? StudentId { get; set; }
}

public sealed class MarkChatReadRequest
{
    /// <summary>Marks messages from the other party as read (max id optionally).</summary>
    public int? UpToMessageId { get; set; }
}

public static class ChatSenderType
{
    public const byte Parent = 0;
    public const byte Teacher = 1;
}
