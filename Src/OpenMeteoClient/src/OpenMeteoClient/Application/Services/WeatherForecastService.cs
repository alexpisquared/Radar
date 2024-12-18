using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenMeteoClient.Application.Interfaces;
using OpenMeteoClient.Domain.Models;
using OpenMeteoClient.Infrastructure.Interfaces;

namespace OpenMeteoClient.Application.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IOpenMeteoApiClient _apiClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherForecastService> _logger;

        public WeatherForecastService(IOpenMeteoApiClient apiClient, IMemoryCache cache, ILogger<WeatherForecastService> logger)
        {
            _apiClient = apiClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<WeatherForecast?> GetForecastAsync(double latitude, double longitude)
        {
            var cacheKey = $"forecast_{latitude}_{longitude}";

            if (_cache.TryGetValue(cacheKey, out WeatherForecast? cachedForecast))
            {
                _logger.LogInformation($"Returning cached forecast for coordinates: {latitude}, {longitude}");
                return cachedForecast;
            }

            _logger.LogInformation($"Fetching weather forecast for coordinates: {latitude}, {longitude}");
            var forecast = await _apiClient.GetForecastAsync(latitude, longitude);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            _cache.Set(cacheKey, forecast, cacheEntryOptions);

            return forecast;
        }
    }
}