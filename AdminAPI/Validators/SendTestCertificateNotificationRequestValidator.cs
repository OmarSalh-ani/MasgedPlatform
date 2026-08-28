using AdminAPI.DTOs.TestCertificate;
using FluentValidation;

namespace AdminAPI.Validators;

public class SendTestCertificateNotificationRequestValidator
    : AbstractValidator<SendTestCertificateNotificationRequestDto>
{
    public SendTestCertificateNotificationRequestValidator()
    {
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
    }
}
