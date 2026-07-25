using AdminAPI.DTOs.QuranCircles;

using FluentValidation;



namespace AdminAPI.Validators;



public class SaveQuranCircleRequestValidator : AbstractValidator<SaveQuranCircleRequestDto>

{

    public SaveQuranCircleRequestValidator()

    {

        RuleFor(x => x.Name)

            .NotEmpty()

            .WithMessage("يرجى إدخال اسم الحلقة")

            .Must(name => !string.IsNullOrWhiteSpace(name))

            .WithMessage("يرجى إدخال اسم الحلقة");

    }

}

