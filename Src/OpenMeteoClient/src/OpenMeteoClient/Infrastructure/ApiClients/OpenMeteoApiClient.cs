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
      var url =
        $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}" +
        $"&current=temperature_2m,apparent_temperature,precipitation,weather_code,wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
        $"&hourly=temperature_2m,apparent_temperature,precipitation_probability,precipitation,weather_code,surface_pressure,wind_speed_10m,wind_direction_10m,wind_gusts_10m" +
        $"&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset,daylight_duration,sunshine_duration&hourly=surface_pressure" +
        $"&timezone=America%2FNew_York" +
        $"&forecast_days=7&past_days=2";
      // Inreractive:                                  https://open-meteo.com/en/docs#current=temperature_2m,apparent_temperature,precipitation,weather_code,wind_speed_10m,wind_direction_10m,wind_gusts_10m&hourly=temperature_2m,apparent_temperature,precipitation_probability,precipitation,weather_code,surface_pressure,wind_speed_10m,wind_direction_10m,wind_gusts_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset,daylight_duration,sunshine_duration&timezone=America%2FNew_York&past_days=2&forecast_days=3
      // API: $"https://api.open-meteo.com/v1/forecast?latitude=43.83&longitude=-79.5&current=temperature_2m,apparent_temperature,precipitation,weather_code,wind_speed_10m,wind_direction_10m,wind_gusts_10m&hourly=temperature_2m,apparent_temperature,precipitation_probability,precipitation,weather_code,surface_pressure,wind_speed_10m,wind_direction_10m,wind_gusts_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset,daylight_duration,sunshine_duration&timezone=America%2FNew_York&past_days=2&forecast_days=3

      Trace.WriteLine(url);

      try
      {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var retVal = JsonConvert.DeserializeObject<WeatherForecast>(content) ?? throw new ArgumentNullException(nameof(content), "■ ■ ■ 654");
        var backToJson = JsonConvert.SerializeObject(retVal);

        Trace.WriteLine(content);
        Trace.WriteLine(backToJson);

        return retVal;
      }
      catch (HttpRequestException ex)
      {
        _logger.LogError(ex, $"Error fetching weather forecast at \n {url}");
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