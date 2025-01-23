using GasNow.Business;
using GasNow.Api.Controllers;
using GasNow.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace GasNow.Tests.Controller
{
    public class GasFeeControllerTests
    {
        private readonly GasFeeController _controller;
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IBusinessFactory> _businessFactoryMock;
        private readonly Mock<GasFeeBusiness> _gasFeeBusinessMock;
        private readonly Mock<IConfiguration> _configuration;

        public GasFeeControllerTests()
        {
            // Mock Redis if needed
            _redisMock = new Mock<IConnectionMultiplexer>(); 
            _businessFactoryMock = new Mock<IBusinessFactory>();

            // Create a mock of GasFeeBusiness
            _gasFeeBusinessMock = new Mock<GasFeeBusiness>();

            // Create a mock IConfiguration
            _configuration = new Mock<IConfiguration>();

            // Setup the factory to return the mocked GasFeeBusiness
            _businessFactoryMock.Setup(factory => factory.Create<GasFeeBusiness>(It.IsAny<object[]>()))
                .Returns(_gasFeeBusinessMock.Object);

            // Create the controller with the mocked factory
            _controller = new GasFeeController(_businessFactoryMock.Object, _redisMock.Object, _configuration.Object);
        }

        [Fact]
        public async Task GetCurrentGasFee_ReturnsOkResult_WhenGasFeeIsNotNull()
        {
            // Arrange
            var expectedGasFee = new GasFeeDto();
            _gasFeeBusinessMock.Setup(b => b.GetCurrentGasFee()).ReturnsAsync(expectedGasFee); // Mock the method to return a Task

            // Act
            var result = await _controller.GetCurrentGasFee(); // Await the result

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualGasFee = Assert.IsType<GasFeeDto>(okResult.Value);
            Assert.Equal(expectedGasFee, actualGasFee);
        }

        [Fact]
        public async Task GetCurrentGasFee_ReturnsNotFound_WhenGasFeeIsNull()
        {
            // Arrange
            _gasFeeBusinessMock.Setup(b => b.GetCurrentGasFee()).ReturnsAsync((GasFeeDto)null); // Mock the method to return null

            // Act
            var result = await _controller.GetCurrentGasFee(); // Await the result

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No gas fee data available.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetGasFeeLastWeek_ReturnsOkResult_WhenGasFeeLastWeekIsNotNull()
        {
            // Arrange
            var expectedGasFeeLastWeek = new List<decimal> { 1.0m, 2.0m, 3.0m }; // Example data
            _gasFeeBusinessMock.Setup(b => b.GetGasFeeLastWeek()).ReturnsAsync(expectedGasFeeLastWeek); // Mock the method to return a Task

            // Act
            var result = await _controller.GetGasFeeLastWeek(); // Await the result

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualGasFeeLastWeek = Assert.IsType<List<decimal>>(okResult.Value);
            Assert.Equal(expectedGasFeeLastWeek, actualGasFeeLastWeek);
        }

        [Fact]
        public async Task GetGasFeeLastWeek_ReturnsNotFound_WhenGasFeeLastWeekIsNullOrEmpty()
        {
            // Arrange
            _gasFeeBusinessMock.Setup(b => b.GetGasFeeLastWeek()).ReturnsAsync(new List<decimal>()); // Mock the method to return an empty list

            // Act
            var result = await _controller.GetGasFeeLastWeek(); // Await the result

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No gas fee data available.", notFoundResult.Value);
        }
    }
}