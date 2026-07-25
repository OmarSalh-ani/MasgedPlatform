using AdminAPI.DTOs.PlanLevels;
using AdminAPI.Models.Enums;
using FluentValidation;

namespace AdminAPI.Validators;

public class SavePlanLevelRequestValidator : AbstractValidator<SavePlanLevelRequestDto>
{
    private static readonly byte[] AllowedUnitTypes =
    [
        (byte)PlanUnitType.Page,
        (byte)PlanUnitType.QuarterPage,
        (byte)PlanUnitType.Line
    ];

    public SavePlanLevelRequestValidator()
    {
        RuleFor(x => x.LevelName)
            .NotEmpty()
            .WithMessage("الاسم مطلوب")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("الاسم مطلوب")
            .MaximumLength(200);

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1)
            .WithMessage("الكمية يجب أن تكون رقمًا موجبًا")
            .LessThanOrEqualTo(1000)
            .WithMessage("الكمية يجب أن تكون رقمًا موجبًا");

        RuleFor(x => x.UnitType)
            .Must(unitType => AllowedUnitTypes.Contains(unitType))
            .WithMessage("نوع القدرة غير صالح");
    }
}
