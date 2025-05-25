using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Shared.DTOs;
using BackEndElog.Application.Queries;
using Moq;
using Xunit;

namespace BackEndElog.Tests.Application.Queries;

public class GetOdometerQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsExpectedResult()
    {
        // Arrange
        var mockService = new Mock<IOdometerService>();
        var handler = new GetOdometerQueryHandler(mockService.Object);

        var query = new GetOdometerQuery
        {
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            IdTms = new List<string> { "123" },
            LicensePlate = new List<string> { "ABC1234" },
            DivisionId = new List<int> { 1 },
            Rows = 10,
            Page = 1
        };

        var expectedResult = new OdometerResultDto
        {
            TotalItems = 1,
            NumberOfRowPage = 10,
            PageActive = 1,
            TotalPages = 1,
            Data = new List<OdometerItemDto>()
        };

        mockService
            .Setup(s => s.GetOdometerDataAsync(It.IsAny<OdometerQueryDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalItems);
    }
}
