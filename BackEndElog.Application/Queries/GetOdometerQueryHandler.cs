using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Shared.DTOs;

namespace BackEndElog.Application.Queries;

public class GetOdometerQueryHandler(IOdometerService service)
{
    public async Task<OdometerResultDto?> HandleAsync(GetOdometerQuery query)
    {
        var dto = new OdometerQueryDto
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            IdTms = query.IdTms,
            LicensePlate = query.LicensePlate,
            DivisionId = query.DivisionId,
            Rows = query.Rows,
            Page = query.Page
        };

        return await service.GetOdometerDataAsync(dto);
    }
}