using AdminAPI.DTOs.AttendanceReport;
using FluentValidation;

namespace AdminAPI.Validators;

public class SendAttendanceWhatsappRequestValidator : AbstractValidator<SendAttendanceWhatsappRequestDto>
{
    public SendAttendanceWhatsappRequestValidator()
    {
        RuleFor(x => x.StudentIds)
            .NotEmpty()
            .WithMessage("يرجى تحديد السجلات المراد إرسال الرسائل لها");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("يرجى كتابة الرسالة أولاً");
    }
}

public class SaveDepartureRequestValidator : AbstractValidator<List<SaveDepartureItemDto>>
{
    public SaveDepartureRequestValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("يرجى تحديد السجلات المراد تسجيل انصرافها");
    }
}
