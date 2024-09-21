using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using OpenMeteoClient.Domain.Models;
using OpenMeteoClient.Infrastructure.ApiClients;
using Xunit;

namespace OpenMeteoClient.Tests
{
    public class OpenMeteoApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<ILogger<OpenMeteoApiClient>> _mockLogger;
        private readonly OpenMeteoApiClient _client;

        public OpenMeteoApiClientTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockLogger = new Mock<ILogger<OpenMeteoApiClient>>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _client = new OpenMeteoApiClient(httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetForecastAsync_SuccessfulResponse_ReturnsParsedForecast()
        {
            // Arrange
            var forecast = new WeatherForecast { Latitude = 43.83, Longitude = -79.5 };
            var jsonResponse = JsonConvert.SerializeObject(forecast);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _client.GetForecastAsync(43.83, -79.5);

            // Assert
            Assert.Equal(forecast.Latitude, result.Latitude);
            Assert.Equal(forecast.Longitude, result.Longitude);
        }

        [Fact]
        public async Task GetForecastAsync_ErrorResponse_ThrowsHttpRequestException()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _client.GetForecastAsync(43.83, -79.5));
        }

        [Fact]
        public async Task GetForecastAsync_NullResponse_ThrowsArgumentNullException()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("null")
                });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.GetForecastAsync(43.83, -79.5));
        }
    }
}