using AdminAPI.DTOs.MasgedSettings;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AdminAPI.Validators;

public partial class FirstTimeSetupRequestValidator : AbstractValidator<FirstTimeSetupRequestDto>
{
    public FirstTimeSetupRequestValidator()
    {
        RuleFor(x => x.MasgedName)
            .NotEmpty()
            .WithMessage("اسم الشركة / المسجد مطلوب")
            .MaximumLength(200)
            .WithMessage("الاسم يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.PrimaryColor)
            .NotEmpty()
            .WithMessage("اللون مطلوب")
            .Must(BeHexColor)
            .WithMessage("اللون يجب أن يكون بصيغة #RRGGBB");

        RuleFor(x => x.Domain)
            .NotEmpty()
            .WithMessage("النطاق مطلوب")
            .MaximumLength(200)
            .WithMessage("النطاق يجب ألا يتجاوز 200 حرف")
            .Must(BeDomainLike)
            .WithMessage("صيغة النطاق غير صحيحة");

        RuleFor(x => x.ParentAppStoreUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.ParentAppStoreUrl));
        RuleFor(x => x.ParentGooglePlayUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.ParentGooglePlayUrl));
        RuleFor(x => x.TeacherAppStoreUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.TeacherAppStoreUrl));
        RuleFor(x => x.TeacherGooglePlayUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.TeacherGooglePlayUrl));

        RuleFor(x => x.AdminName)
            .NotEmpty()
            .WithMessage("اسم مدير النظام مطلوب")
            .MaximumLength(200)
            .WithMessage("اسم المدير يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.AdminEmail)
            .NotEmpty()
            .WithMessage("بريد المدير مطلوب")
            .EmailAddress()
            .WithMessage("صيغة البريد غير صحيحة")
            .MaximumLength(200)
            .WithMessage("البريد يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.AdminPassword)
            .NotEmpty()
            .WithMessage("كلمة مرور المدير مطلوبة")
            .MinimumLength(6)
            .WithMessage("كلمة المرور يجب ألا تقل عن 6 أحرف")
            .MaximumLength(500)
            .WithMessage("كلمة المرور طويلة جداً");
    }

    private static bool BeHexColor(string? value) =>
        !string.IsNullOrWhiteSpace(value) && HexColorRegex().IsMatch(value.Trim());

    private static bool BeDomainLike(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        var domain = value.Trim().ToLowerInvariant()
            .Replace("https://", string.Empty)
            .Replace("http://", string.Empty)
            .TrimEnd('/');
        return DomainRegex().IsMatch(domain);
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexColorRegex();

    [GeneratedRegex(@"^[a-z0-9]([a-z0-9\-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9\-]*[a-z0-9])?)+$")]
    private static partial Regex DomainRegex();
}
