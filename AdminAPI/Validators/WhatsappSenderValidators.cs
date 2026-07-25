using AdminAPI.DTOs.WhatsappSender;
using FluentValidation;

namespace AdminAPI.Validators;

public class SendWhatsappSenderRequestValidator : AbstractValidator<SendWhatsappSenderRequestDto>
{
    public SendWhatsappSenderRequestValidator()
    {
        RuleFor(x => x.StudentIds).NotEmpty().WithMessage("يرجى تحديد الطلاب المراد إرسال الرسائل لهم");
        RuleFor(x => x.Message).NotEmpty().WithMessage("يرجى كتابة الرسالة أولاً");
    }
}
