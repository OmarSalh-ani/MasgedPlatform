using AdminAPI.DTOs.TeacherSendNotes;
using FluentValidation;

namespace AdminAPI.Validators;

public class UpdateTeacherSendNoteRequestValidator : AbstractValidator<UpdateTeacherSendNoteRequestDto>
{
    public UpdateTeacherSendNoteRequestValidator()
    {
        RuleFor(x => x.Note)
            .NotEmpty()
            .WithMessage("يرجى كتابة نص الملاحظة")
            .Must(note => !string.IsNullOrWhiteSpace(note))
            .WithMessage("يرجى كتابة نص الملاحظة");
    }
}
