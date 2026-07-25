using AdminAPI.DTOs.Competitions;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveCompetitionRequestValidator : AbstractValidator<SaveCompetitionRequestDto>
{
    public SaveCompetitionRequestValidator()
    {
        RuleFor(x => x.Title)
            .Must(t => !string.IsNullOrWhiteSpace(t));

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.LinkUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.LinkUrl));
    }
}
