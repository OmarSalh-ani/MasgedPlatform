using AdminAPI.DTOs.StudentPlan;
using FluentValidation;

namespace AdminAPI.Validators;

public class CreateStudentPlanRequestValidator : AbstractValidator<CreateStudentPlanRequestDto>
{
    public CreateStudentPlanRequestValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().WithMessage("يرجى إدخال اسم الخطة.");
    }
}

public class SaveStudentPlanRequestValidator : AbstractValidator<SaveStudentPlanRequestDto>
{
    public SaveStudentPlanRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.StudentId.HasValue || x.StudentIds.Count > 0)
            .WithMessage("يرجى اختيار طالب واحد على الأقل.");
    }
}

public class UpdateStudentPlanItemRequestValidator : AbstractValidator<UpdateStudentPlanItemRequestDto>
{
    public UpdateStudentPlanItemRequestValidator()
    {
        RuleFor(x => x.EditKey).NotEmpty();
        RuleFor(x => x.SurahId).GreaterThan(0);
        RuleFor(x => x)
            .Must(x => x.SurahId > 1000 || (x.FromAyahNumber > 0 && x.ToAyahNumber > 0))
            .WithMessage("يرجى تعبئة السورة و من آية وإلى آية.");
    }
}
