using AdminAPI.DTOs.ParentsFollowup;
using AdminAPI.Services;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveParentsFollowupRequestValidator : AbstractValidator<SaveParentsFollowupRequestDto>
{
    private static readonly string[] GenderValues = ["ذكر", "أنثى"];
    private static readonly string[] MaritalValues = ["متزوج / ة", "متوفي /ة", "مطلق / ة", "أعزب"];
    private static readonly string[] YesNoValues = ["نعم", "لا"];

    public SaveParentsFollowupRequestValidator()
    {
        RuleFor(x => x.StudentName)
            .NotEmpty()
            .WithMessage("الرجاء إدخال اسم الطالب");

        RuleFor(x => x.Birthdate)
            .NotNull()
            .WithMessage("الرجاء إدخال تاريخ الميلاد");

        RuleFor(x => x.StudentGender)
            .NotEmpty()
            .WithMessage("الرجاء أختيار الجنس")
            .Must(v => GenderValues.Contains(v))
            .WithMessage("الرجاء أختيار الجنس");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("الرجاء ادخال العنوان");

        RuleFor(x => x.FatherName)
            .NotEmpty()
            .WithMessage("الرجاء ادخال اسم ولي الامر");

        RuleFor(x => x.FatherPhone)
            .NotEmpty()
            .WithMessage("الرجاء ادخال رقم ولي الامر");

        RuleFor(x => x.MaritalStatus)
            .NotEmpty()
            .WithMessage("الرجاء أختيار الحالة الأجتماعية")
            .Must(v => MaritalValues.Contains(v))
            .WithMessage("الرجاء أختيار الحالة الأجتماعية");

        RuleFor(x => x.HealthCondition)
            .NotEmpty()
            .WithMessage("الرجاء اختيار الحالة الصحية والتعليمية")
            .Must(v => YesNoValues.Contains(v))
            .WithMessage("الرجاء اختيار الحالة الصحية والتعليمية");

        RuleFor(x => x.LearningDifficulties)
            .NotEmpty()
            .WithMessage("الرجاء أختيار هل يعاني الطالب من صعوبات تعليمية أو سلوكية")
            .Must(v => YesNoValues.Contains(v))
            .WithMessage("الرجاء أختيار هل يعاني الطالب من صعوبات تعليمية أو سلوكية");

        When(x => x.Photo is { Length: > 0 }, () =>
        {
            RuleFor(x => x.Photo)
                .Must(HasAllowedExtension)
                .WithMessage("نوع الملف غير مدعوم. يرجى اختيار صورة بصيغة JPG أو JPEG أو PNG")
                .Must(f => f!.Length <= ParentsFollowupPhotoStorage.MaxBytes)
                .WithMessage("حجم الملف كبير جداً. يرجى اختيار صورة أقل من 1 ميجابايت")
                .Must(HasValidAspectRatio)
                .WithMessage(ParentsFollowupPhotoValidation.AspectErrorMessage);
        });
    }

    private static bool HasValidAspectRatio(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return true;

        using var stream = file.OpenReadStream();
        if (!ParentsFollowupPhotoValidation.TryGetImageDimensions(stream, out var width, out var height))
            return false;

        return ParentsFollowupPhotoValidation.HasValidAspectRatio(width, height);
    }

    private static bool HasAllowedExtension(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return true;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return ParentsFollowupPhotoStorage.AllowedExtensions.Contains(extension);
    }
}
