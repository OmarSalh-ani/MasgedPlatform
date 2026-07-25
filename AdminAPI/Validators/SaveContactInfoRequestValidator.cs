using AdminAPI.DTOs.ContactInfo;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveContactInfoRequestValidator : AbstractValidator<SaveContactInfoRequestDto>
{
    public SaveContactInfoRequestValidator()
    {
        RuleFor(x => x.ContactType)
            .NotEmpty()
            .WithMessage("نوع التواصل مطلوب")
            .Must(type => !string.IsNullOrWhiteSpace(type))
            .WithMessage("نوع التواصل مطلوب")
            .MaximumLength(50);

        RuleFor(x => x.Label)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Label));

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("القيمة مطلوبة")
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("القيمة مطلوبة")
            .MaximumLength(500);
    }
}
