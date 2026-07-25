using AdminAPI.DTOs.Auth;
using FluentValidation;

namespace AdminAPI.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("يرجى إدخال كلمة المرور الحالية");

        RuleFor(x => x.NewPassword)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("يرجى إدخال كلمة المرور الجديدة");

        RuleFor(x => x.ConfirmPassword)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("يرجى تأكيد كلمة المرور");

        RuleFor(x => x)
            .Must(x => x.NewPassword == x.ConfirmPassword)
            .WithMessage("كلمة المرور غير متطابقة")
            .When(x => !string.IsNullOrWhiteSpace(x.NewPassword) && !string.IsNullOrWhiteSpace(x.ConfirmPassword));
    }
}
