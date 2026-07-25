using AdminAPI.DTOs.WomansActivities;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveWomanActivityRequestValidator : AbstractValidator<SaveWomanActivityRequestDto>
{
    public SaveWomanActivityRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("يرجى كتابة اسم النشاط أولاً")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("يرجى كتابة اسم النشاط أولاً")
            .MaximumLength(500);
    }
}
