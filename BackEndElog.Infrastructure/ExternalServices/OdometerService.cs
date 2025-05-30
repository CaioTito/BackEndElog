using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Infrastructure.Resilience;
using BackEndElog.Shared.Configurations;
using BackEndElog.Shared.DTOs;
using BackEndElog.Shared.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace BackEndElog.Infrastructure.ExternalServices;

public class OdometerService : IOdometerService
{
    private readonly HttpClient _client;
    private readonly string _odometerPath;
    private readonly ILogger<OdometerService> _logger;
    private readonly AsyncPolicy<Result<OdometerResultDto?>> _retryPolicy;

    public OdometerService(
        IHttpClientFactory factory,
        IOptions<ElogApiSettings> options,
        ILogger<OdometerService> logger)
    {
        _client = factory.CreateClient("ElogClient");
        _odometerPath = options.Value.OdometerPath;
        _logger = logger;
        _retryPolicy = PolicyFactory.CreateRetryPolicy<OdometerResultDto>(_logger);
    }

    public async Task<Result<OdometerResultDto?>> GetOdometerDataAsync(OdometerQueryDto query)
    {
        return await _retryPolicy.ExecuteAsync(async (context) =>
        {
            try
            {
                var requestUri = BuildRequestUri(query);
                var response = await _client.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    return HandleUnsuccessfulResponse(response, context);
                }

                var result = await response.Content.ReadFromJsonAsync<OdometerResultDto>();

                if (result == null)
                {
                    _logger.LogWarning("Resposta nula da API externa.");
                    return Result<OdometerResultDto?>.Failure(new Error(502, "Resposta inválida da API externa."));
                }

                result.Data = result.Data
                    .Select(item =>
                    {
                        if (IsLicensePlate(item.VehicleIdTms))
                            item.VehicleIdTms = string.Empty;
                        return item;
                    })
                    .ToList();

                return Result<OdometerResultDto?>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado em GetOdometerDataAsync: {Message}", ex.Message);
                return Result<OdometerResultDto?>.Failure(new Error(500, "Erro interno ao processar a requisição."));
            }

        }, new Context());
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

    private static Result<OdometerResultDto?> HandleUnsuccessfulResponse(HttpResponseMessage response, Context context)
    {
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            if (response.Headers.TryGetValues("Retry-After", out var values) &&
                int.TryParse(values.FirstOrDefault(), out var seconds))
            {
                context["RetryAfter"] = TimeSpan.FromSeconds(seconds);
            }
            else
            {
                context["RetryAfter"] = TimeSpan.FromSeconds(5);
            }

            return Result<OdometerResultDto?>.Failure(new Error(429, "Muitas requisições. Aguarde e tente novamente."));
        }

        var code = (int)response.StatusCode;
        var description = $"Erro HTTP {code} - {response.ReasonPhrase}";
        return Result<OdometerResultDto?>.Failure(new Error(code, description));
    }

    private static bool IsLicensePlate(string input)
    {
        return !string.IsNullOrEmpty(input) &&
               Regex.IsMatch(input, @"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$", RegexOptions.IgnoreCase);
    }
}