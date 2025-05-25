using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Shared.Configurations;
using BackEndElog.Shared.DTOs;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Web;

namespace BackEndElog.Infrastructure.ExternalServices;

public class OdometerService : IOdometerService
{
    private readonly HttpClient _client;
    private readonly string _odometerPath;

    public OdometerService(IHttpClientFactory factory, IOptions<ElogApiSettings> options)
    {
        _client = factory.CreateClient("ElogClient");
        _odometerPath = options.Value.OdometerPath;
    }

    public async Task<OdometerResultDto?> GetOdometerDataAsync(OdometerQueryDto query)
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

        return await response.Content.ReadFromJsonAsync<OdometerResultDto>();
    }
}