using BackEndElog.Shared.DTOs;
using BackEndElog.Shared.Results;

namespace BackEndElog.Infrastructure.Interfaces;

public interface IOdometerService
{
    Task<Result<OdometerResultDto?>> GetOdometerDataAsync(OdometerQueryDto query);
}
