using AdminAPI.DTOs.PublicIndex;
using FluentValidation;
using Masged.WhatsApp;

namespace AdminAPI.Validators;

public class SubmitPublicRegistrationRequestValidator : AbstractValidator<SubmitPublicRegistrationRequestDto>
{
    public SubmitPublicRegistrationRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("يرجى إدخال الاسم");

        RuleFor(x => x.ParentPhoneCountryIso)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("يرجى اختيار رمز الدولة");

        RuleFor(x => x.ParentPhone1)
            .NotEmpty()
            .WithMessage("يرجى إدخال رقم الجوال");

        RuleFor(x => x.WomanActivityTypeId)
            .GreaterThan(0)
            .WithMessage("يرجى اختيار نوع النشاط");

        RuleFor(x => x)
            .Must(x => x.Mode.Equals("wregister", StringComparison.OrdinalIgnoreCase)
                ? x.Age is >= 5
                : x.Birthdate.HasValue)
            .WithMessage("يرجى إدخال تاريخ الميلاد أو العمر");

        RuleFor(x => x)
            .Must(x => !x.Mode.Equals("wregister", StringComparison.OrdinalIgnoreCase)
                || x.Age is null
                || x.Age >= 5)
            .WithMessage("يجب أن لا يقل العمر عن 5 سنوات");

        RuleFor(x => x)
            .Must(HasValidParentPhone)
            .WithMessage("رقم الجوال غير صالح");

        RuleFor(x => x)
            .Must(x => string.IsNullOrWhiteSpace(x.ParentPhone2) || HasValidParentPhone2(x))
            .WithMessage("رقم الجوال البديل غير صالح");
    }

    private static bool HasValidParentPhone(SubmitPublicRegistrationRequestDto request)
    {
        var digits = DigitsOnly(request.ParentPhone1);
        if (string.IsNullOrEmpty(digits))
            return false;

        if (request.ParentPhoneCountryIso.Equals("KW", StringComparison.OrdinalIgnoreCase))
            return digits.Length == 8 && digits.All(char.IsDigit);

        return digits.Length is >= 7 and <= 15 && digits.All(char.IsDigit);
    }

    private static bool HasValidParentPhone2(SubmitPublicRegistrationRequestDto request)
    {
        var digits = DigitsOnly(request.ParentPhone2);
        if (string.IsNullOrEmpty(digits))
            return false;

        var iso = string.IsNullOrWhiteSpace(request.ParentPhone2CountryIso)
            ? "KW"
            : request.ParentPhone2CountryIso;

        if (iso.Equals("KW", StringComparison.OrdinalIgnoreCase))
            return digits.Length == 8 && digits.All(char.IsDigit);

        return digits.Length is >= 7 and <= 15 && digits.All(char.IsDigit);
    }

    private static string DigitsOnly(string? value) =>
        PhoneNormalizer.ToEnglishDigits(value);
}
