using AdminAPI.DTOs.EventPages;
using AdminAPI.DTOs.PublicEventPages;
using AdminAPI.Models;

namespace AdminAPI.Services;

public static class EventPageMapper
{
    public static EventPageListItemDto ToListItem(EventPage page) => new()
    {
        Id = page.Id,
        ActivityName = page.ActivityName,
        Slug = page.Slug,
        CourseTitle = page.CourseTitle,
        ImageUrl = EventPageImageStorage.NormalizeImageUrl(page.ImageUrl),
        IsPublished = page.IsPublished,
        IsRegistrationOpen = page.IsRegistrationOpen,
        CreatedAt = page.CreatedAt,
    };

    public static EventPageDto ToDto(EventPage page) => new()
    {
        Id = page.Id,
        ActivityName = page.ActivityName,
        Slug = page.Slug,
        CourseTitle = page.CourseTitle,
        InvitationText = page.InvitationText,
        MosqueName = page.MosqueName,
        SubjectText = page.SubjectText,
        DateText = page.DateText,
        TimeText = page.TimeText,
        ExtraNotes = page.ExtraNotes,
        SupervisorsText = page.SupervisorsText,
        ContactPhone = page.ContactPhone,
        SocialAccounts = page.SocialAccounts,
        LocationNote = page.LocationNote,
        ImageUrl = EventPageImageStorage.NormalizeImageUrl(page.ImageUrl),
        IsPublished = page.IsPublished,
        IsRegistrationOpen = page.IsRegistrationOpen,
        CreatedAt = page.CreatedAt,
        Tracks = page.Tracks
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.Id)
            .Select(t => new EventPageTrackDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                SortOrder = t.SortOrder,
            })
            .ToList(),
        FormFields = page.FormFields
            .OrderBy(f => f.SortOrder)
            .ThenBy(f => f.Id)
            .Select(ToFormFieldDto)
            .ToList(),
    };

    public static PublicEventPageDto ToPublicDto(EventPage page) => new()
    {
        Id = page.Id,
        ActivityName = page.ActivityName,
        Slug = page.Slug,
        CourseTitle = page.CourseTitle,
        InvitationText = page.InvitationText,
        MosqueName = page.MosqueName,
        SubjectText = page.SubjectText,
        DateText = page.DateText,
        TimeText = page.TimeText,
        ExtraNotes = page.ExtraNotes,
        SupervisorsText = page.SupervisorsText,
        ContactPhone = page.ContactPhone,
        SocialAccounts = page.SocialAccounts,
        LocationNote = page.LocationNote,
        ImageUrl = EventPageImageStorage.NormalizeImageUrl(page.ImageUrl),
        IsRegistrationOpen = page.IsRegistrationOpen,
        Tracks = page.Tracks
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.Id)
            .Select(t => new PublicEventPageTrackDto
            {
                Title = t.Title,
                Description = t.Description,
                SortOrder = t.SortOrder,
            })
            .ToList(),
        FormFields = page.FormFields
            .OrderBy(f => f.SortOrder)
            .ThenBy(f => f.Id)
            .Select(f => new PublicEventPageFormFieldDto
            {
                Id = f.Id,
                Label = f.Label,
                FieldType = f.FieldType,
                IsRequired = f.IsRequired,
                SortOrder = f.SortOrder,
                Options = EventPageJsonParser.ParseOptions(f.OptionsJson),
            })
            .ToList(),
    };

    private static EventPageFormFieldDto ToFormFieldDto(EventPageFormField field) => new()
    {
        Id = field.Id,
        Label = field.Label,
        FieldType = field.FieldType,
        IsRequired = field.IsRequired,
        SortOrder = field.SortOrder,
        Options = EventPageJsonParser.ParseOptions(field.OptionsJson),
    };
}
