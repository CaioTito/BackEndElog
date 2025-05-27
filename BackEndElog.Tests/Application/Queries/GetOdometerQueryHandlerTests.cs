using BackEndElog.Infrastructure.Interfaces;
using BackEndElog.Shared.DTOs;
using BackEndElog.Application.Queries;
using Moq;
using Xunit;
using BackEndElog.Shared.Results;
using FluentValidation;
using FluentValidation.Results;

namespace BackEndElog.Tests.Application.Queries;

public class GetOdometerQueryHandlerTests
{
    private readonly Mock<IOdometerService> _mockService;
    private readonly Mock<IValidator<GetOdometerQuery>> _mockValidator;
    private readonly GetOdometerQueryHandler _handler;

    public GetOdometerQueryHandlerTests()
    {
        _mockService = new Mock<IOdometerService>();
        _mockValidator = new Mock<IValidator<GetOdometerQuery>>();

        _mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<GetOdometerQuery>(), default))
            .ReturnsAsync(new ValidationResult());

        _handler = new GetOdometerQueryHandler(_mockService.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task HandleAsync_ReturnsExpectedResult()
    {
        // Arrange
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

        var expectedResult = Result<OdometerResultDto?>.Success(new OdometerResultDto
        {
            TotalItems = 1,
            NumberOfRowPage = 10,
            PageActive = 1,
            TotalPages = 1,
            Data = new List<OdometerItemDto>()
        });

        _mockService
            .Setup(s => s.GetOdometerDataAsync(It.IsAny<OdometerQueryDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.TotalItems);
    }
}