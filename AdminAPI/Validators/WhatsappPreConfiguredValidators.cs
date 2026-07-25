using AdminAPI.DTOs.WhatsappPreConfigured;
using FluentValidation;

namespace AdminAPI.Validators;

public class UpdateWhatsappPreConfiguredRequestValidator
    : AbstractValidator<UpdateWhatsappPreConfiguredRequestDto>
{
    public UpdateWhatsappPreConfiguredRequestValidator()
    {
        RuleFor(x => x.WhatsappMessage).NotEmpty().WithMessage("الرسالة مطلوبة");
    }
}
