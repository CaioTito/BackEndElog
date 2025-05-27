using BackEndElog.Application.Queries;
using FluentValidation;

namespace BackEndElog.Application.Validators;

public class GetOdometerQueryValidator : AbstractValidator<GetOdometerQuery>
{
    public GetOdometerQueryValidator()
    {
        RuleFor(b => b.StartDate)
            .NotNull().WithMessage("Start Date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Start Date cannot be in the future");

        RuleFor(b => b.EndDate)
            .NotNull().WithMessage("End Date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("End Date cannot be in the future");

        RuleFor(b => b)
            .Must(b => b.StartDate < b.EndDate)
            .WithMessage("Start Date must be before End Date");

        RuleFor(b => b)
            .Must(b => (b.EndDate - b.StartDate).TotalDays <= 90)
            .WithMessage("The date range cannot exceed 90 days");
    }
}
