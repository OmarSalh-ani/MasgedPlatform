using AdminAPI.DTOs.EventPages;
using AdminAPI.Models;

namespace AdminAPI.Services;

public static class EventPageLandingBinder
{
    public static void Apply(EventPage page, SaveEventPageRequestDto request)
    {
        page.ActivityName = EventPageText.Required(request.ActivityName);
        page.Slug = EventPageText.NormalizeSlug(request.Slug);
        page.CourseTitle = EventPageText.Required(request.CourseTitle);
        page.InvitationText = EventPageText.Optional(request.InvitationText);
        page.MosqueName = EventPageText.Optional(request.MosqueName);
        page.SubjectText = EventPageText.Optional(request.SubjectText);
        page.DateText = EventPageText.Optional(request.DateText);
        page.TimeText = EventPageText.Optional(request.TimeText);
        page.ExtraNotes = EventPageText.Optional(request.ExtraNotes);
        page.SupervisorsText = EventPageText.Optional(request.SupervisorsText);
        page.ContactPhone = EventPageText.Optional(request.ContactPhone);
        page.SocialAccounts = EventPageText.Optional(request.SocialAccounts);
        page.LocationNote = EventPageText.Optional(request.LocationNote);
        page.IsPublished = request.IsPublished;
        page.IsRegistrationOpen = request.IsRegistrationOpen;
    }

    public static void ReplaceTracks(EventPage page, IReadOnlyList<SaveEventPageTrackItemDto> tracks)
    {
        page.Tracks.Clear();
        foreach (var track in tracks)
        {
            page.Tracks.Add(new EventPageTrack
            {
                Title = EventPageText.Required(track.Title),
                Description = EventPageText.Optional(track.Description),
                SortOrder = track.SortOrder,
            });
        }
    }

    public static void ReplaceFields(EventPage page, IReadOnlyList<SaveEventPageFormFieldItemDto> fields)
    {
        page.FormFields.Clear();
        foreach (var field in fields)
        {
            page.FormFields.Add(new EventPageFormField
            {
                Label = EventPageText.Required(field.Label),
                FieldType = EventPageText.Required(field.FieldType),
                IsRequired = field.IsRequired,
                SortOrder = field.SortOrder,
                OptionsJson = EventPageFieldTypes.IsSelect(field.FieldType)
                    ? EventPageJsonParser.SerializeOptions(field.Options)
                    : null,
            });
        }
    }
}
