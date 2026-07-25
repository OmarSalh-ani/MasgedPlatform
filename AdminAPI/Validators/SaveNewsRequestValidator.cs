using AdminAPI.DTOs.News;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveNewsRequestValidator : AbstractValidator<SaveNewsRequestDto>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public SaveNewsRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("العنوان مطلوب")
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("العنوان مطلوب")
            .MaximumLength(300);

        RuleFor(x => x.Image)
            .Must(HasAllowedExtension)
            .When(x => x.Image is { Length: > 0 })
            .WithMessage("الامتدادات المسموحة: jpg, jpeg, png, gif, webp");
    }

    private static bool HasAllowedExtension(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return true;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
