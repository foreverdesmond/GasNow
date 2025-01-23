using System.Net;
using System.Text;
using System.Text.Json;
using GasNow.ExternalApis;
using GasNow.Module;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace GasNow.Tests.ExternalApis
{
    public class InfuraApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly InfuraApiService _infuraApiService;

        public InfuraApiServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApiUrls:InfuraApi", "https://api.infura.io/v1/jsonrpc/mainnet" }
                })
                .Build();

            _infuraApiService = new InfuraApiService(_configuration);
        }

        [Fact]
        public async Task GetBaseFeeHistory_ReturnsListOfDecimals_WhenResponseIsValid()
        {
            // Arrange
            var blockNumber = 12345678;
            var jsonResponse = JsonSerializer.Serialize(new JsonRpcResponse
            {
                Result = new Dictionary<string, object>
                {
                    { "baseFeePerGas", new JsonElement[] { JsonDocument.Parse("0x1dcd65000").RootElement, JsonDocument.Parse("0x1dcd65001").RootElement } }
                }
            });

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
            var result = await _infuraApiService.GetBaseFeeHistory(blockNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(0.25m, result[0]); // 0x1dcd65000 = 80000000000 / 10^18
            Assert.Equal(0.25m, result[1]); // 0x1dcd65001 = 80000000001 / 10^18
        }

        [Fact]
        public async Task GetBaseFeeHistory_ReturnsEmptyList_WhenResultIsNull()
        {
            // Arrange
            var blockNumber = 12345678;
            var jsonResponse = JsonSerializer.Serialize(new JsonRpcResponse
            {
                Result = null
            });

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
            var result = await _infuraApiService.GetBaseFeeHistory(blockNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBaseFeeHistory_ReturnsEmptyList_WhenBaseFeePerGasIsNotArray()
        {
            // Arrange
            var blockNumber = 12345678;
            var jsonResponse = JsonSerializer.Serialize(new JsonRpcResponse
            {
                Result = new Dictionary<string, object>
                {
                    { "baseFeePerGas", "not an array" }
                }
            });

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
            var result = await _infuraApiService.GetBaseFeeHistory(blockNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBaseFeeHistory_LogsError_WhenExceptionOccurs()
        {
            // Arrange
            var blockNumber = 12345678;
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _infuraApiService.GetBaseFeeHistory(blockNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Ensure it returns an empty list on error
        }
    }
}
