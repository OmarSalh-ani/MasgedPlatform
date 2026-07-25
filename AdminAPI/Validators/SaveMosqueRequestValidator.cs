using AdminAPI.DTOs.Mosques;

using FluentValidation;



namespace AdminAPI.Validators;



public class SaveMosqueRequestValidator : AbstractValidator<SaveMosqueRequestDto>

{

    public SaveMosqueRequestValidator()

    {

        RuleFor(x => x.Name)

            .NotEmpty()

            .WithMessage("اسم المسجد مطلوب")

            .Must(name => !string.IsNullOrWhiteSpace(name))

            .WithMessage("اسم المسجد مطلوب")

            .MaximumLength(200);



        RuleFor(x => x.GoogleMapsUrl)

            .MaximumLength(500)

            .When(x => !string.IsNullOrWhiteSpace(x.GoogleMapsUrl));

    }

}

