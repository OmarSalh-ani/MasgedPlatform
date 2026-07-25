using AdminAPI.DTOs.TeacherSendNotes;
using FluentValidation;

namespace AdminAPI.Validators;

public class CreateTeacherSendNotesRequestValidator : AbstractValidator<CreateTeacherSendNotesRequestDto>
{
    public CreateTeacherSendNotesRequestValidator()
    {
        RuleFor(x => x.TeacherIds)
            .NotEmpty()
            .WithMessage("يرجى اختيار معلم واحد على الأقل");

        RuleFor(x => x.Note)
            .NotEmpty()
            .WithMessage("يرجى كتابة نص الملاحظة")
            .Must(note => !string.IsNullOrWhiteSpace(note))
            .WithMessage("يرجى كتابة نص الملاحظة");
    }
}
