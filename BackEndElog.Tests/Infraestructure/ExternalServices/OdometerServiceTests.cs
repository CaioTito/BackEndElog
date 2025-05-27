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
    [Fact]
    public async Task GetOdometerDataAsync_ReturnsValidData()
    {
        // Arrange
        var expectedDto = new OdometerResultDto
        {
            TotalItems = 1,
            Data = new List<OdometerItemDto>()
        };

        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedDto))
            });

        var httpClient = new HttpClient(mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://api-elog-client.azurewebsites.net/api/v1/")
        };

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var mockOptions = new Mock<IOptions<ElogApiSettings>>();
        mockOptions.Setup(o => o.Value).Returns(new ElogApiSettings
        {
            OdometerPath = "Vehicles/TrackerOdometer"
        });

        var loggerMock = new Mock<ILogger<OdometerService>>();

        var service = new OdometerService(factoryMock.Object, mockOptions.Object, loggerMock.Object);

        var query = new OdometerQueryDto
        {
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow
        };

        // Act
        var result = await service.GetOdometerDataAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Value.TotalItems);
    }
}
