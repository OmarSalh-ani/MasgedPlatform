using AdminAPI.DTOs.About;
using FluentValidation;

namespace AdminAPI.Validators;

public class UpdateAboutRequestValidator : AbstractValidator<UpdateAboutRequestDto>
{
    public UpdateAboutRequestValidator()
    {
        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.MapsUrl)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.MapsUrl));
    }
}
