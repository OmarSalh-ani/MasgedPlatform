using AdminAPI.DTOs.Student;

using FluentValidation;



namespace AdminAPI.Validators;



public class SaveStudentRequestValidator : AbstractValidator<SaveStudentRequestDto>

{

    public SaveStudentRequestValidator()

    {

        RuleFor(x => x.FullName)

            .NotEmpty()

            .WithMessage("يرجى إدخال اسم الطالب");



        RuleFor(x => x.FatherPhone)

            .NotEmpty()

            .WithMessage("يرجى إدخال رقم الهاتف");



        RuleFor(x => x.StudentGender)

            .Must(g => g is "ذكر" or "أنثى")

            .WithMessage("يرجى اختيار الجنس");

    }

}

