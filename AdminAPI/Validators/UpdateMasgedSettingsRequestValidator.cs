using AdminAPI.DTOs.MasgedSettings;
using FluentValidation;

namespace AdminAPI.Validators;

public class UpdateMasgedSettingsRequestValidator : AbstractValidator<UpdateMasgedSettingsRequestDto>
{
    public UpdateMasgedSettingsRequestValidator()
    {
        RuleFor(x => x.MasgedName)
            .NotEmpty()
            .WithMessage("اسم المسجد مطلوب")
            .MaximumLength(200)
            .WithMessage("اسم المسجد يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.ParentAppStoreUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.ParentAppStoreUrl));
        RuleFor(x => x.ParentGooglePlayUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.ParentGooglePlayUrl));
        RuleFor(x => x.TeacherAppStoreUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.TeacherAppStoreUrl));
        RuleFor(x => x.TeacherGooglePlayUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.TeacherGooglePlayUrl));
        RuleFor(x => x.PrimaryColor)
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("اللون يجب أن يكون بصيغة #RRGGBB")
            .When(x => !string.IsNullOrWhiteSpace(x.PrimaryColor));
    }
}
