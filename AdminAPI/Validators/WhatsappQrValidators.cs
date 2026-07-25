using AdminAPI.DTOs.WhatsappQr;
using FluentValidation;

namespace AdminAPI.Validators;

public class CreateWhatsappSessionRequestValidator : AbstractValidator<CreateWhatsappSessionRequestDto>
{
    public CreateWhatsappSessionRequestValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("رقم الهاتف مطلوب");
    }
}
