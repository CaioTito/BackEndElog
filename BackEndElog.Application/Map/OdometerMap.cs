using BackEndElog.Application.Queries;
using BackEndElog.Shared.DTOs;

namespace BackEndElog.Application.Map;

public static class OdometerMap
{
    public static OdometerQueryDto ToDto(GetOdometerQuery query)
    {
        return new OdometerQueryDto
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            IdTms = query.IdTms,
            LicensePlate = query.LicensePlate,
            DivisionId = query.DivisionId,
            Rows = query.Rows,
            Page = query.Page
        };
    }
}
