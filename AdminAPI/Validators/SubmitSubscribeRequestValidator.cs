using AdminAPI.DTOs.Subscribe;
using FluentValidation;

namespace AdminAPI.Validators;

public class SubmitSubscribeRequestValidator : AbstractValidator<SubmitSubscribeRequestDto>
{
    public SubmitSubscribeRequestValidator()
    {
        RuleFor(x => x.FullName)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("يرجى إدخال الاسم الثلاثي");

        RuleFor(x => x.Mobile)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("يرجى إدخال رقم الموبايل")
            .Must(v => v.Trim().Length == 8 && v.Trim().All(char.IsDigit))
            .WithMessage("يرجى إدخال رقم الموبايل الصحيح (8 أرقام)");
    }
}
