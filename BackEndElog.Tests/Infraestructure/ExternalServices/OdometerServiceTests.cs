using BackEndElog.Infrastructure.ExternalServices;
using BackEndElog.Shared.Configurations;
using BackEndElog.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace BackEndElog.Tests.Infraestructure.ExternalServices;

public class OdometerServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IOptions<ElogApiSettings>> _mockOptions;
    private readonly Mock<ILogger<OdometerService>> _mockLogger;
    private readonly OdometerService _service;

    public OdometerServiceTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://api-elog-client.azurewebsites.net/api/v1/")
        };

        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        _mockOptions = new Mock<IOptions<ElogApiSettings>>();
        _mockOptions.Setup(o => o.Value).Returns(new ElogApiSettings
        {
            OdometerPath = "Vehicles/TrackerOdometer"
        });

        _mockLogger = new Mock<ILogger<OdometerService>>();

        _service = new OdometerService(_mockHttpClientFactory.Object, _mockOptions.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetOdometerDataAsync_ReturnsValidData()
    {
        // Arrange
        var expectedDto = new OdometerResultDto
        {
            TotalItems = 1,
            Data = new List<OdometerItemDto>()
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedDto))
            });

        var query = new OdometerQueryDto
        {
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.GetOdometerDataAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Value.TotalItems);
    }
}