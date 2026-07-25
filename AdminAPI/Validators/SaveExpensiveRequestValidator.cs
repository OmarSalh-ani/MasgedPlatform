using AdminAPI.DTOs.Expensives;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveExpensiveRequestValidator : AbstractValidator<SaveExpensiveRequestDto>
{
    public SaveExpensiveRequestValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("سبب الصرف مطلوب")
            .MaximumLength(500);

        RuleFor(x => x.Supplier)
            .NotEmpty()
            .WithMessage("اسم المورد مطلوب")
            .MaximumLength(250);

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("القيمة يجب أن تكون صفراً أو أكثر");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
