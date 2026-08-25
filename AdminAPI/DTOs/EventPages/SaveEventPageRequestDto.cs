namespace AdminAPI.DTOs.EventPages;

public class SaveEventPageRequestDto
{
    public string ActivityName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public string? InvitationText { get; set; }
    public string? MosqueName { get; set; }
    public string? SubjectText { get; set; }
    public string? DateText { get; set; }
    public string? TimeText { get; set; }
    public string? ExtraNotes { get; set; }
    public string? SupervisorsText { get; set; }
    public string? ContactPhone { get; set; }
    public string? SocialAccounts { get; set; }
    public string? LocationNote { get; set; }
    public bool IsPublished { get; set; } = true;
    public bool IsRegistrationOpen { get; set; } = true;
    public IFormFile? Image { get; set; }
    public string? TracksJson { get; set; }
    public string? FieldsJson { get; set; }
}
