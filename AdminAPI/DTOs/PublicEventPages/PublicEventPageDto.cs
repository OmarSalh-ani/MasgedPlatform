namespace AdminAPI.DTOs.PublicEventPages;

public class PublicEventPageDto
{
    public int Id { get; set; }
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
    public string? ImageUrl { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public List<PublicEventPageTrackDto> Tracks { get; set; } = [];
    public List<PublicEventPageFormFieldDto> FormFields { get; set; } = [];
}
