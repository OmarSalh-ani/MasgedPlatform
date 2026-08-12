using AdminAPI.DTOs.CircleVisitRating;
using AdminAPI.Services;
using FluentValidation;

namespace AdminAPI.Validators;

public class CreateCircleVisitRatingRequestValidator : AbstractValidator<CreateCircleVisitRatingRequestDto>
{
    public CreateCircleVisitRatingRequestValidator()
    {
        RuleFor(x => x.TeacherId)
            .GreaterThan(0)
            .WithMessage("يرجى اختيار المعلم");

        RuleFor(x => x.QuranCircleId)
            .GreaterThan(0)
            .WithMessage("يرجى اختيار الحلقة");

        RuleFor(x => x.VisitDate)
            .NotEmpty()
            .WithMessage("يرجى إدخال تاريخ الزيارة");

        RuleFor(x => x.VisitTime)
            .NotEmpty()
            .WithMessage("يرجى إدخال وقت الزيارة")
            .Must(t => TimeSpan.TryParse(t, out _))
            .WithMessage("وقت الزيارة غير صالح");

        RuleFor(x => x.Items)
            .Must(items => items.Count == CircleVisitRatingCriteria.Criteria.Length)
            .WithMessage("يجب تعبئة جميع عناصر التقييم");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Sequence)
                .InclusiveBetween(1, CircleVisitRatingCriteria.Criteria.Length)
                .WithMessage("تسلسل البند غير صالح");

            item.RuleFor(i => i.Criterion)
                .NotEmpty()
                .WithMessage("البند مطلوب")
                .Must(c => CircleVisitRatingCriteria.Criteria.Contains(c.Trim()))
                .WithMessage("البند غير صالح");

            item.RuleFor(i => i.Rating)
                .NotEmpty()
                .WithMessage("التقييم مطلوب")
                .Must(r => CircleVisitRatingCriteria.Ratings.Contains(r.Trim()))
                .WithMessage("قيمة التقييم غير صالحة");

            item.RuleFor(i => i.Notes)
                .MaximumLength(1000)
                .WithMessage("الملاحظات يجب ألا تتجاوز 1000 حرف");
        });
    }
}
