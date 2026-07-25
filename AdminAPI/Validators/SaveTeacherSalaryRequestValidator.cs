using AdminAPI.DTOs.TeacherSalaries;
using FluentValidation;

namespace AdminAPI.Validators;

public class SaveTeacherSalaryRequestValidator : AbstractValidator<SaveTeacherSalaryRequestDto>
{
    public SaveTeacherSalaryRequestValidator()
    {
        RuleFor(x => x.TeacherId).GreaterThan(0).WithMessage("يرجى اختيار المعلم");
        RuleFor(x => x.Month).InclusiveBetween(1, 12).WithMessage("يرجى اختيار الشهر");
        RuleFor(x => x.Year).GreaterThan(0).WithMessage("يرجى اختيار السنة");
        RuleFor(x => x.BaseSalary).GreaterThan(0).WithMessage("يرجى إدخال الراتب الأساسي بشكل صحيح");
    }
}
