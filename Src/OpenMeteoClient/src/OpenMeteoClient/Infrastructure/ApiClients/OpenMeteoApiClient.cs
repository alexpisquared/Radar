using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenMeteoClient.Domain.Models;
using OpenMeteoClient.Infrastructure.Interfaces;

namespace OpenMeteoClient.Infrastructure.ApiClients
{
  public class OpenMeteoApiClient : IOpenMeteoApiClient
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenMeteoApiClient> _logger;

    public OpenMeteoApiClient(HttpClient httpClient, ILogger<OpenMeteoApiClient> logger)
    {
      _httpClient = httpClient;
      _logger = logger;
    }

    public async Task<WeatherForecast> GetForecastAsync(double latitude, double longitude)
    {
      try
      {
        var response = await _httpClient.GetAsync(
                       //$"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m,precipitation_probability,precipitation,weather_code,wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
                       $"https://api.open-meteo.com/v1/forecast?latitude=43.83&longitude=-79.5&hourly=temperature_2m,precipitation_probability,precipitation,weather_code,wind_speed_10m,wind_direction_10m,wind_gusts_10m"
          );
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Trace.WriteLine(content);
        var retVal =  JsonConvert.DeserializeObject<WeatherForecast>(content) ?? throw new ArgumentNullException(nameof(content), "■ ■ ■ 654");
        var backToJson = JsonConvert.SerializeObject(retVal);
        Trace.WriteLine(backToJson);
        return retVal;
      }
      catch (HttpRequestException ex)
      {
        _logger.LogError(ex, "Error fetching weather forecast");
        throw;
      }
      catch (JsonException ex)
      {
        _logger.LogError(ex, "Error deserializing weather forecast response");
        throw;
      }
    }
  }
}