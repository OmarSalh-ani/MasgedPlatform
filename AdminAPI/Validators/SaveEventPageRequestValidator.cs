using System.Text.RegularExpressions;
using AdminAPI.DTOs.EventPages;
using AdminAPI.Models;
using AdminAPI.Services;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveEventPageRequestValidator : AbstractValidator<SaveEventPageRequestDto>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
    private static readonly Regex SlugPattern = new(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);

    public SaveEventPageRequestValidator()
    {
        RuleFor(x => x.ActivityName)
            .NotEmpty().WithMessage("اسم النشاط مطلوب")
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("رابط الصفحة مطلوب")
            .MaximumLength(120)
            .Must(slug => SlugPattern.IsMatch((slug ?? string.Empty).Trim().ToLowerInvariant()))
            .WithMessage("الرابط يجب أن يكون أحرفاً إنجليزية صغيرة وأرقاماً وشرطات فقط");

        RuleFor(x => x.CourseTitle)
            .NotEmpty().WithMessage("عنوان الدورة مطلوب")
            .MaximumLength(300);

        RuleFor(x => x.InvitationText).MaximumLength(500);
        RuleFor(x => x.MosqueName).MaximumLength(300);
        RuleFor(x => x.SubjectText).MaximumLength(1000);
        RuleFor(x => x.DateText).MaximumLength(300);
        RuleFor(x => x.TimeText).MaximumLength(300);
        RuleFor(x => x.SupervisorsText).MaximumLength(1000);
        RuleFor(x => x.ContactPhone).MaximumLength(50);
        RuleFor(x => x.SocialAccounts).MaximumLength(200);
        RuleFor(x => x.LocationNote).MaximumLength(500);

        RuleFor(x => x.Image)
            .Must(HasAllowedExtension)
            .When(x => x.Image is { Length: > 0 })
            .WithMessage("الامتدادات المسموحة: jpg, jpeg, png, gif");

        RuleFor(x => x.TracksJson).Must(BeValidTracks).WithMessage("صيغة المسارات غير صحيحة");
        RuleFor(x => x.FieldsJson).Must(BeValidFields).WithMessage("صيغة حقول النموذج غير صحيحة");
        RuleFor(x => x.FieldsJson).Custom(ValidateFields);
    }

    private static bool HasAllowedExtension(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return true;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private static bool BeValidTracks(string? json)
    {
        try
        {
            var tracks = EventPageJsonParser.ParseTracks(json);
            return tracks.All(t => !string.IsNullOrWhiteSpace(t.Title) && t.Title.Trim().Length <= 300);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static bool BeValidFields(string? json)
    {
        try
        {
            _ = EventPageJsonParser.ParseFields(json);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static void ValidateFields(string? json, ValidationContext<SaveEventPageRequestDto> context)
    {
        List<SaveEventPageFormFieldItemDto> fields;
        try
        {
            fields = EventPageJsonParser.ParseFields(json);
        }
        catch (ArgumentException)
        {
            return;
        }

        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            if (string.IsNullOrWhiteSpace(field.Label))
                context.AddFailure($"Label", $"عنوان الحقل مطلوب (حقل {i + 1})");

            if (!EventPageFieldTypes.All.Contains(field.FieldType))
                context.AddFailure("FieldType", $"نوع الحقل غير صحيح (حقل {i + 1})");

            if (EventPageFieldTypes.IsSelect(field.FieldType)
                && field.Options.All(string.IsNullOrWhiteSpace))
            {
                context.AddFailure("Options", $"خيارات الحقل مطلوبة (حقل {i + 1})");
            }
        }
    }
}
