using AdminAPI.DTOs.WorkDays;
using FluentValidation;

namespace AdminAPI.Validators;

public class UpdateWorkDaysRequestValidator : AbstractValidator<UpdateWorkDaysRequestDto>
{
    public UpdateWorkDaysRequestValidator()
    {
        RuleFor(x => x.DayNumbers)
            .NotEmpty()
            .WithMessage("يجب اختيار يوم عمل واحد على الأقل");

        RuleForEach(x => x.DayNumbers)
            .InclusiveBetween(0, 6)
            .WithMessage("رقم اليوم يجب أن يكون بين 0 و 6");
    }
}
