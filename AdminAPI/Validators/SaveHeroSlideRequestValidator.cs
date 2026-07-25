using AdminAPI.DTOs.HeroSlides;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveHeroSlideRequestValidator : AbstractValidator<SaveHeroSlideRequestDto>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public SaveHeroSlideRequestValidator()
    {
        RuleForEach(x => x.Images)
            .Must(HasAllowedExtension)
            .When(x => x.Images.Count > 0)
            .WithMessage("يرجى اختيار ملفات صورة فقط (JPG, PNG, GIF, WebP).");
    }

    private static bool HasAllowedExtension(IFormFile file)
    {
        if (file.Length == 0)
            return true;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
