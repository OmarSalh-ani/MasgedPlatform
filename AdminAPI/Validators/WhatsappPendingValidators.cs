using AdminAPI.DTOs.WhatsappPending;
using FluentValidation;

namespace AdminAPI.Validators;

public class DeleteWhatsappPendingRequestValidator : AbstractValidator<DeleteWhatsappPendingRequestDto>
{
    public DeleteWhatsappPendingRequestValidator()
    {
        RuleFor(x => x.Ids).NotEmpty().WithMessage("لم تحدد أي رسائل.");
    }
}
