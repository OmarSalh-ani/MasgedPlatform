namespace MasgedTeacherMobileAPI.Dtos;

public class ParentQuestionDto
{
    public int Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string? TeacherReply { get; set; }
    public bool IsRead { get; set; }
}

public class UpdateTeacherReplyRequestDto
{
    public string Reply { get; set; } = string.Empty;
}
