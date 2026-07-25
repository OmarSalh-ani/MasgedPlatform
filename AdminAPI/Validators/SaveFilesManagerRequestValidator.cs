using AdminAPI.DTOs.FilesManager;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveFilesManagerRequestValidator : AbstractValidator<SaveFilesManagerRequestDto>
{
    public SaveFilesManagerRequestValidator()
    {
        RuleFor(x => x.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("يرجى إدخال اسم الملف");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("يرجى اختيار ملف للرفع")
            .Must(file => file is { Length: > 0 })
            .WithMessage("يرجى اختيار ملف للرفع");
    }
}
