using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using OpenMeteoClient.Application.Services;
using OpenMeteoClient.Domain.Models;
using OpenMeteoClient.Infrastructure.Interfaces;
using Xunit;

namespace OpenMeteoClient.Tests
{
    public class WeatherForecastServiceTests
    {
        private readonly Mock<IOpenMeteoApiClient> _mockApiClient;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<WeatherForecastService>> _mockLogger;
        private readonly WeatherForecastService _service;

        public WeatherForecastServiceTests()
        {
            _mockApiClient = new Mock<IOpenMeteoApiClient>();
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<WeatherForecastService>>();
            _service = new WeatherForecastService(_mockApiClient.Object, _mockCache.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetForecastAsync_CacheHit_ReturnsCachedForecast()
        {
            // Arrange
            var cachedForecast = new WeatherForecast { Latitude = 43.83, Longitude = -79.5 };
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedForecast)).Returns(true);

            // Act
            var result = await _service.GetForecastAsync(43.83, -79.5);

            // Assert
            Assert.Equal(cachedForecast, result);
            _mockApiClient.Verify(x => x.GetForecastAsync(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }

        [Fact]
        public async Task GetForecastAsync_CacheMiss_CallsApiAndCachesForecast()
        {
            // Arrange
            var forecast = new WeatherForecast { Latitude = 43.83, Longitude = -79.5 };
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<WeatherForecast>.IsAny)).Returns(false);
            _mockApiClient.Setup(x => x.GetForecastAsync(It.IsAny<double>(), It.IsAny<double>())).ReturnsAsync(forecast);

            // Act
            var result = await _service.GetForecastAsync(43.83, -79.5);

            // Assert
            Assert.Equal(forecast, result);
            _mockApiClient.Verify(x => x.GetForecastAsync(43.83, -79.5), Times.Once);
            _mockCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Once);
        }
    }
}