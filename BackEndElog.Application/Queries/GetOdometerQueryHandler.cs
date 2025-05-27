using BackEndElog.Application.Map;
using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Shared.DTOs;
using BackEndElog.Shared.Results;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace BackEndElog.Application.Queries;

public class GetOdometerQueryHandler(IOdometerService service, IValidator<GetOdometerQuery> validator)
{
    public async Task<Result<OdometerResultDto?>> HandleAsync(GetOdometerQuery query)
    {
        var validationResult = await validator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            var combinedMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            var error = new Error(code: 400, description: combinedMessage);
            return Result<OdometerResultDto?>.Failure(error);
        }

        return await service.GetOdometerDataAsync(OdometerMap.ToDto(query));
    }
}