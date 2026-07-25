#nullable disable

namespace MasgedParentMobileAPI.Models;

public partial class ParentTeacherChatMessage
{
    public int Id { get; set; }
    public string ParentPhone { get; set; }
    public int TeacherId { get; set; }
    public byte SenderType { get; set; }
    public string MessageText { get; set; }
    public int? StudentId { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsReadByParent { get; set; }
    public bool IsReadByTeacher { get; set; }
}
