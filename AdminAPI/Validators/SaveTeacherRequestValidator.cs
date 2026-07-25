using AdminAPI.DTOs.Teachers;
using AdminAPI.Services;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveTeacherRequestValidator : AbstractValidator<SaveTeacherRequestDto>
{
    public SaveTeacherRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("يرجى إدخال أسم المعلم");
        RuleFor(x => x.Email).NotEmpty().WithMessage("يرجى إدخال البريد الإلكتروني");
        RuleFor(x => x.Image)
            .Must(file => file is null || TeacherImageStorage.AllowedExtensions.Contains(
                Path.GetExtension(file.FileName).ToLowerInvariant()))
            .When(x => x.Image is { Length: > 0 })
            .WithMessage("الامتدادات المسموحة: jpg, jpeg, png, gif, webp");
    }
}
