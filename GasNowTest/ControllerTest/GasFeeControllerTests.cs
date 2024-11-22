using GasNow.Business;
using GasNow.Api.Controllers;
using GasNow.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StackExchange.Redis;

namespace GasNow.Tests.Controller
{
    public class GasFeeControllerTests
    {
        private readonly GasFeeController _controller;
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IBusinessFactory> _businessFactoryMock;
        private readonly Mock<GasFeeBusiness> _gasFeeBusinessMock;

        public GasFeeControllerTests()
        {
            _redisMock = new Mock<IConnectionMultiplexer>(); // Mock Redis if needed
            _businessFactoryMock = new Mock<IBusinessFactory>();

            // Create a mock of GasFeeBusiness
            _gasFeeBusinessMock = new Mock<GasFeeBusiness>();

            // Setup the factory to return the mocked GasFeeBusiness
            _businessFactoryMock.Setup(factory => factory.Create<GasFeeBusiness>(It.IsAny<object[]>()))
                .Returns(_gasFeeBusinessMock.Object);

            // Create the controller with the mocked factory
            _controller = new GasFeeController(_redisMock.Object, _businessFactoryMock.Object);
        }

        [Fact]
        public void GetCurrentGasFee_ReturnsOkResult_WhenGasFeeIsNotNull()
        {
            // Arrange
            var expectedGasFee = new GasFeeDto();
            _gasFeeBusinessMock.Setup(b => b.GetCurrentGasFee()).ReturnsAsync(expectedGasFee); // Mock the method to return a Task

            // Act
            var result = _controller.GetCurrentGasFee(); // No await here

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualGasFee = Assert.IsType<GasFeeDto>(okResult.Value);
            Assert.Equal(expectedGasFee, actualGasFee);
        }

        [Fact]
        public void GetCurrentGasFee_ReturnsNotFound_WhenGasFeeIsNull()
        {
            // Arrange
            _gasFeeBusinessMock.Setup(b => b.GetCurrentGasFee()).ReturnsAsync((GasFeeDto)null); // Mock the method to return null

            // Act
            var result = _controller.GetCurrentGasFee(); // No await here

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No gas fee data available.", notFoundResult.Value);
        }
    }
}