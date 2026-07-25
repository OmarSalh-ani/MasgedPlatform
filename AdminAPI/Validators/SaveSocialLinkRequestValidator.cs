using AdminAPI.DTOs.SocialLink;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveSocialLinkRequestValidator : AbstractValidator<SaveSocialLinkRequestDto>
{
    public SaveSocialLinkRequestValidator()
    {
        RuleFor(x => x.PlatformName)
            .NotEmpty()
            .WithMessage("اسم المنصة مطلوب")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("اسم المنصة مطلوب")
            .MaximumLength(100);

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("الرابط مطلوب")
            .Must(url => !string.IsNullOrWhiteSpace(url))
            .WithMessage("الرابط مطلوب")
            .MaximumLength(500);

        RuleFor(x => x.IconClass)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.IconClass));
    }
}
