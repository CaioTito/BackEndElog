using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Shared.Configurations;
using BackEndElog.Shared.DTOs;
using BackEndElog.Shared.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Web;
using static System.Net.WebRequestMethods;

namespace BackEndElog.Infrastructure.ExternalServices;

public class OdometerService : IOdometerService
{
    private readonly HttpClient _client;
    private readonly string _odometerPath;
    private readonly ILogger<OdometerService> _logger;

    public OdometerService(IHttpClientFactory factory, IOptions<ElogApiSettings> options, ILogger<OdometerService> logger)
    {
        _client = factory.CreateClient("ElogClient");
        _odometerPath = options.Value.OdometerPath;
        _logger = logger;
    }

    public async Task<Result<OdometerResultDto?>> GetOdometerDataAsync(OdometerQueryDto query)
    {
        try
        {
            var builder = new UriBuilder($"{_client.BaseAddress}{_odometerPath}");
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            parameters["StartDate"] = query.StartDate.ToString("o");
            parameters["EndDate"] = query.EndDate.ToString("o");

            if (query.IdTms != null)
                foreach (var id in query.IdTms)
                    parameters.Add("IdTms", id);

            if (query.LicensePlate != null)
                foreach (var plate in query.LicensePlate)
                    parameters.Add("LicensePlate", plate);

            if (query.DivisionId != null)
                foreach (var div in query.DivisionId)
                    parameters.Add("DivisionId", div.ToString());

            if (query.Rows.HasValue)
                parameters["Rows"] = query.Rows.Value.ToString();

            if (query.Page.HasValue)
                parameters["Page"] = query.Page.Value.ToString();

            builder.Query = parameters.ToString();
            var response = await _client.GetAsync(builder.ToString());

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OdometerResultDto>();

            if (result == null)
            {
                _logger.LogWarning("Received null response for Odometer data.");
                return Result<OdometerResultDto?>.Failure(new Error("Received null response for Odometer data."));
            }

            return Result<OdometerResultDto?>.Success(result);
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "GetOdometerDataAsync: {Message}", httpEx.StatusCode);
            var code = httpEx.StatusCode.HasValue ? (int)httpEx.StatusCode.Value : 0;
            var statusCode = httpEx.StatusCode.HasValue ? httpEx.StatusCode.Value.ToString() : "Erro na requisição";
            return Result<OdometerResultDto?>.Failure(new Error(code, statusCode));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOdometerDataAsync: {Message}", ex.Message);
            return Result<OdometerResultDto?>.Failure(new Error(ex.Message));
        }
    }
}