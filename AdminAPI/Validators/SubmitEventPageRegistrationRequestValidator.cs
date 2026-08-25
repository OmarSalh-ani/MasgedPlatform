using AdminAPI.DTOs.PublicEventPages;
using FluentValidation;

namespace AdminAPI.Validators;

public class SubmitEventPageRegistrationRequestValidator
    : AbstractValidator<SubmitEventPageRegistrationRequestDto>
{
    public SubmitEventPageRegistrationRequestValidator()
    {
        RuleFor(x => x.Answers).NotNull();
        RuleForEach(x => x.Answers).ChildRules(answer =>
        {
            answer.RuleFor(a => a.FieldId).GreaterThan(0);
        });
    }
}
