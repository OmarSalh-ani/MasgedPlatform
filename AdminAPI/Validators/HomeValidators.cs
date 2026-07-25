using AdminAPI.DTOs.Home;
using FluentValidation;

namespace AdminAPI.Validators;

public class TransferHomeStudentsRequestValidator : AbstractValidator<TransferHomeStudentsRequestDto>
{
    public TransferHomeStudentsRequestValidator()
    {
        RuleFor(x => x.StudentIds).NotEmpty().WithMessage("يرجى تحديد الطلاب المراد نقلهم");
        RuleFor(x => x.CircleId).GreaterThan(0).WithMessage("يرجى اختيار الحلقة الجديدة");
    }
}

public class RemoveHomeStudentsFromCircleRequestValidator
    : AbstractValidator<RemoveHomeStudentsFromCircleRequestDto>
{
    public RemoveHomeStudentsFromCircleRequestValidator()
    {
        RuleFor(x => x.StudentIds).NotEmpty().WithMessage("يرجى تحديد الطلاب المراد إزالتهم من الحلقات");
    }
}

public class CreateHomeCircleRequestValidator : AbstractValidator<CreateHomeCircleRequestDto>
{
    public CreateHomeCircleRequestValidator()
    {
        RuleFor(x => x.CircleName).NotEmpty().WithMessage("يرجى إدخال اسم الحلقة");
        RuleFor(x => x.TeacherId).GreaterThan(0).WithMessage("يرجى اختيار المعلم");
    }
}

public class UpdateHomeRegistrationRequestValidator : AbstractValidator<UpdateHomeRegistrationRequestDto>
{
    public UpdateHomeRegistrationRequestValidator()
    {
        RuleFor(x => x).Must(_ => true);
    }
}
