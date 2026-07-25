using AdminAPI.DTOs.Activity;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveActivityRequestValidator : AbstractValidator<SaveActivityRequestDto>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];

    public SaveActivityRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("العنوان مطلوب")
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("العنوان مطلوب")
            .MaximumLength(200);

        RuleFor(x => x.Image)
            .Must(HasAllowedExtension)
            .When(x => x.Image is { Length: > 0 })
            .WithMessage("الامتدادات المسموحة: jpg, jpeg, png, gif");
    }

    private static bool HasAllowedExtension(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return true;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
