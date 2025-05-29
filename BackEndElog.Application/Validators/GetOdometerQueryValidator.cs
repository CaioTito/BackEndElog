using BackEndElog.Application.Queries;
using FluentValidation;

namespace BackEndElog.Application.Validators;

public class GetOdometerQueryValidator : AbstractValidator<GetOdometerQuery>
{
    public GetOdometerQueryValidator()
    {
        RuleFor(b => b.StartDate)
            .NotNull().NotEmpty().WithMessage("Data inicial é obrigatória")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Data inicial, não pode ser uma data futura");

        RuleFor(b => b.EndDate)
            .NotNull().NotEmpty().WithMessage("Data final é obrigatória")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Data final, não pode ser uma data futura");

        RuleFor(b => b)
            .Must(b => b.StartDate < b.EndDate)
            .WithMessage("Data inicial precisa ser anterior a data final");

        RuleFor(b => b)
            .Must(b => (b.EndDate - b.StartDate).TotalDays <= 90)
            .WithMessage("O período máximo entre as datas não deve ultrapassar 90 dias");
    }
}
