using AdminAPI.DTOs.QuranCircles;
using FluentValidation;

namespace AdminAPI.Validators;

public class DeleteCirclePlansRequestValidator : AbstractValidator<DeleteCirclePlansRequestDto>
{
    public DeleteCirclePlansRequestValidator()
    {
        RuleFor(x => x.CircleIds)
            .NotEmpty()
            .WithMessage("يرجى تحديد حلقة واحدة على الأقل");

        RuleForEach(x => x.CircleIds)
            .GreaterThan(0)
            .WithMessage("معرف الحلقة غير صالح");
    }
}
