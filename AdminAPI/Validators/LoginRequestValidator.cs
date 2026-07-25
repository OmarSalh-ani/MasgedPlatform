using AdminAPI.DTOs.Auth;
using FluentValidation;

namespace AdminAPI.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("يرجى تعبئة جميع الحقول!");

        RuleFor(x => x.Password)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("يرجى تعبئة جميع الحقول!");
    }
}
