using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Shared.Configurations;
using BackEndElog.Shared.DTOs;
using BackEndElog.Shared.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Web;

namespace BackEndElog.Infrastructure.ExternalServices;

public class OdometerService : IOdometerService
{
    private readonly HttpClient _client;
    private readonly string _odometerPath;
    private readonly ILogger<OdometerService> _logger;

    public OdometerService(
        IHttpClientFactory factory,
        IOptions<ElogApiSettings> options,
        ILogger<OdometerService> logger)
    {
        _client = factory.CreateClient("ElogClient");
        _odometerPath = options.Value.OdometerPath;
        _logger = logger;
    }

    public async Task<Result<OdometerResultDto?>> GetOdometerDataAsync(OdometerQueryDto query)
    {
        try
        {
            var requestUri = BuildRequestUri(query);
            var response = await _client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OdometerResultDto>();

            if (result == null)
            {
                _logger.LogWarning("Resposta nula da API externa.");
                return Result<OdometerResultDto?>.Failure(new Error(502, "Resposta inválida da API externa."));
            }

            return Result<OdometerResultDto?>.Success(result);
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "Erro na requisição HTTP: {Message}", httpEx.Message);

            var statusCode = httpEx.StatusCode.HasValue ? (int)httpEx.StatusCode.Value : 502;
            var description = httpEx.StatusCode.HasValue
                ? $"Erro HTTP {(int)httpEx.StatusCode.Value} - {httpEx.StatusCode}"
                : "Erro ao se comunicar com a API externa.";

            return Result<OdometerResultDto?>.Failure(new Error(statusCode, description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado em GetOdometerDataAsync: {Message}", ex.Message);

            return Result<OdometerResultDto?>.Failure(new Error(500, "Erro interno ao processar a requisição."));
        }
    }

    private Uri BuildRequestUri(OdometerQueryDto query)
    {
        var builder = new UriBuilder($"{_client.BaseAddress}{_odometerPath}");
        var parameters = HttpUtility.ParseQueryString(string.Empty);

        parameters["StartDate"] = query.StartDate.ToString("o");
        parameters["EndDate"] = query.EndDate.ToString("o");

        if (query.IdTms != null)
        {
            foreach (var id in query.IdTms)
                parameters.Add("IdTms", id);
        }

        if (query.LicensePlate != null)
        {
            foreach (var plate in query.LicensePlate)
                parameters.Add("LicensePlate", plate);
        }

        if (query.DivisionId != null)
        {
            foreach (var div in query.DivisionId)
                parameters.Add("DivisionId", div.ToString());
        }

        if (query.Rows.HasValue)
            parameters["Rows"] = query.Rows.Value.ToString();

        if (query.Page.HasValue)
            parameters["Page"] = query.Page.Value.ToString();

        builder.Query = parameters.ToString();
        return builder.Uri;
    }
}