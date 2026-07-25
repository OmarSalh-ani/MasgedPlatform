using AdminAPI.DTOs.PushNotifications;
using FluentValidation;

namespace AdminAPI.Validators;

public class SendAdminPushNotificationRequestValidator : AbstractValidator<SendAdminPushNotificationRequestDto>
{
    private static readonly HashSet<string> ValidAudiences =
        new(StringComparer.OrdinalIgnoreCase) { "teachers", "parents" };

    public SendAdminPushNotificationRequestValidator()
    {
        RuleFor(x => x.Audience)
            .NotEmpty()
            .WithMessage("يرجى اختيار نوع الجمهور")
            .Must(a => ValidAudiences.Contains(a.Trim()))
            .WithMessage("نوع الجمهور غير صالح");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("يرجى كتابة عنوان الإشعار")
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("يرجى كتابة عنوان الإشعار")
            .MaximumLength(100)
            .WithMessage("العنوان طويل جداً");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("يرجى كتابة نص الإشعار")
            .Must(body => !string.IsNullOrWhiteSpace(body))
            .WithMessage("يرجى كتابة نص الإشعار")
            .MaximumLength(500)
            .WithMessage("نص الإشعار طويل جداً");

        When(x => x.Audience.Equals("teachers", StringComparison.OrdinalIgnoreCase) && !x.TargetAll, () =>
        {
            RuleFor(x => x.TeacherIds)
                .NotEmpty()
                .WithMessage("يرجى اختيار معلم واحد على الأقل");
        });

        When(x => x.Audience.Equals("parents", StringComparison.OrdinalIgnoreCase) && !x.TargetAll, () =>
        {
            RuleFor(x => x.StudentIds)
                .NotEmpty()
                .WithMessage("يرجى اختيار طالب واحد على الأقل");
        });
    }
}
