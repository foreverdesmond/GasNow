using System.Net;
using System.Text;
using System.Text.Json;
using GasNow.Dto;
using GasNow.Module;
using GasNow.ExternalApis;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace GasNow.Tests.ExternalApis
{
    public class EtherscanApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly EtherscanApiService _etherscanApiService;

        public EtherscanApiServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApiUrls:EtherscanApi", "https://api.etherscan.io/api" }
                })
                .Build();

            _etherscanApiService = new EtherscanApiService(_configuration);
        }

        [Fact]
        public async Task GetCurrentGasFeeAsync_ReturnsGasFeeDto_WhenResponseIsValid()
        {
            // Arrange
            var gasApiResponse = new GasApiResponse
            {
                Status = "1",
                Result = new GasResult
                {
                    LastBlock = 12345678,
                    SafeGasPrice = Convert.ToDecimal(50),
                    SuggestBaseFee = Convert.ToDecimal(10),
                    FastGasPrice = Convert.ToDecimal(100),
                    ProposeGasPrice = Convert.ToDecimal(80),
                    GasUsedRatio = "0.5"
                }
            };

            var jsonResponse = JsonSerializer.Serialize(gasApiResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            // Act
            var result = await _etherscanApiService.GetCurrentGasFeeAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12345678, result.BlockNumber);
            Assert.Equal(50, result.SlowMaxFee);
            Assert.Equal(40, result.SlowPriorityFee);
            Assert.Equal(80, result.NormalMaxFee);
            Assert.Equal(70, result.NormalPriorityFee);
            Assert.Equal(100, result.FastMaxFee);
            Assert.Equal(90, result.FastPriorityFee);
        }

        [Fact]
        public async Task GetCurrentGasFeeAsync_ReturnsNull_WhenStatusIsNot1()
        {
            // Arrange
            var gasApiResponse = new GasApiResponse
            {
                Status = "0",
                Result = null
            };

            var jsonResponse = JsonSerializer.Serialize(gasApiResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            // Act
            var result = await _etherscanApiService.GetCurrentGasFeeAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrentGasFeeAsync_ReturnsNull_WhenResponseIsNull()
        {
            // Arrange
            var gasApiResponse = new GasApiResponse
            {
                Status = "1",
                Result = null
            };

            var jsonResponse = JsonSerializer.Serialize(gasApiResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            // Act
            var result = await _etherscanApiService.GetCurrentGasFeeAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrentPriceAsync_ReturnsPriceDto_WhenResponseIsValid()
        {
            // Arrange
            var priceApiResponse = new PriceApiResponse
            {
                Status = "1",
                Result = new PriceResult
                {
                    EthBtc = Convert.ToDecimal("0.05"),
                    EthUsd = Convert.ToDecimal("2000"),
                    EthBtcTimestamp = 1620000000,
                    EthUsdTimestamp = 1620000000
                }
            };

            var jsonResponse = JsonSerializer.Serialize(priceApiResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            // Act
            var result = await _etherscanApiService.GetCurrentPriceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Convert.ToDecimal(0.05), result.EthBtcPrice);
            Assert.Equal(Convert.ToDecimal(2000), result.EthUsdPrice);
        }

        [Fact]
        public async Task GetCurrentPriceAsync_ReturnsNull_WhenStatusIsNot1()
        {
            // Arrange
            var priceApiResponse = new PriceApiResponse
            {
                Status = "0",
                Result = null
            };

            var jsonResponse = JsonSerializer.Serialize(priceApiResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            // Act
            var result = await _etherscanApiService.GetCurrentPriceAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrentPriceAsync_ReturnsPriceDto_WhenResponseIsNull()
        {
            // Arrange
            var priceApiResponse = new PriceApiResponse
            {
                Status = "1",
                Result = null
            };

            var jsonResponse = JsonSerializer.Serialize(priceApiResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            // Act
            var result = await _etherscanApiService.GetCurrentPriceAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCurrentGasFeeAsync_LogsError_WhenExceptionOccurs()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _etherscanApiService.GetCurrentGasFeeAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GasFeeDto>(result); // Ensure it returns a GasFeeDto even on error
        }

        [Fact]
        public async Task GetCurrentPriceAsync_LogsError_WhenExceptionOccurs()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _etherscanApiService.GetCurrentPriceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PriceDto>(result); // Ensure it returns a PriceDto even on error
        }
    }
}
