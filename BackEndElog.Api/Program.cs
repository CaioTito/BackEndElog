using BackEndElog.Application.Queries;
using BackEndElog.Application.Validators;
using BackEndElog.Infrastructure.ExternalServices;
using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Shared.Configurations;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddValidatorsFromAssemblyContaining<GetOdometerQueryValidator>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("ElogClient", (provider, client) =>
{
    var elogConfig = provider.GetRequiredService<IOptions<ElogApiSettings>>().Value;

    client.BaseAddress = new Uri(elogConfig.BaseUrl);

    if (!string.IsNullOrWhiteSpace(elogConfig.AuthorizationToken))
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", elogConfig.AuthorizationToken);
    }
});

builder.Services.AddScoped<IOdometerService, OdometerService>();
builder.Services.AddScoped<GetOdometerQueryHandler>();

builder.Services.Configure<ElogApiSettings>(
    builder.Configuration.GetSection("ElogApi"));

builder.Services.AddHttpClient("ElogClient", (provider, client) =>
{
    var config = provider.GetRequiredService<IOptions<ElogApiSettings>>().Value;

    client.BaseAddress = new Uri(config.BaseUrl);
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", config.AuthorizationToken);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
