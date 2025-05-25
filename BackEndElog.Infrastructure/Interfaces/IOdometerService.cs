using BackEndElog.Shared.DTOs;

namespace BackEndElog.Infrastructure.Interfaces;

public interface IOdometerService
{
    Task<OdometerResultDto?> GetOdometerDataAsync(OdometerQueryDto query);
}
