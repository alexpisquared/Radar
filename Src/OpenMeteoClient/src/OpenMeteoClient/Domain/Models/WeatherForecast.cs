using Newtonsoft.Json;

namespace OpenMeteoClient.Domain.Models;

public class WeatherForecast
{
  public double Latitude { get; set; }
  public double Longitude { get; set; }
  [JsonProperty("generationtime_ms")] public double GenerationTimeMs { get; set; }
  [JsonProperty("utc_offset_seconds")] public int UtcOffsetSeconds { get; set; }
  public string? Timezone { get; set; }
  [JsonProperty("timezone_abbreviation")] public string? TimezoneAbbreviation { get; set; }
  public double Elevation { get; set; }
  [JsonProperty("current_units")] public CurrentUnits? CurrentUnits { get; set; }
  public CurrentData? Current { get; set; }
  [JsonProperty("hourly_units")] public HourlyUnits? HourlyUnits { get; set; }
  public HourlyData? Hourly { get; set; }
  [JsonProperty("daily_units")] public DailyUnits? DailyUnits { get; set; }
  public DailyData? Daily { get; set; }
}
public class CurrentUnits
{
  public string? Time { get; set; }

  [JsonProperty("temperature_2m")]
  public string? Temperature2m { get; set; }

  [JsonProperty("precipitation_probability")]
  public string? PrecipitationProbability { get; set; }

  public string? Precipitation { get; set; }

  [JsonProperty("weather_code")]
  public string? WeatherCode { get; set; }

  [JsonProperty("wind_speed_10m")]
  public string? WindSpeed10m { get; set; }

  [JsonProperty("wind_direction_10m")]
  public string? WindDirection10m { get; set; }

  [JsonProperty("wind_gusts_10m")]
  public string? WindGusts10m { get; set; }
}
public class CurrentData
{
  public DateTime Time { get; set; }
  [JsonProperty("temperature_2m")] public double Temperature2m { get; set; }
  [JsonProperty("apparent_temperature")] public double ApparentTemperature { get; set; }
  [JsonProperty("precipitation_probability")] public int PrecipitationProbability { get; set; }
  public double Precipitation { get; set; }
  [JsonProperty("weather_code")] public int WeatherCode { get; set; }
  [JsonProperty("wind_speed_10m")] public double WindSpeed10m { get; set; }
  [JsonProperty("wind_direction_10m")] public int WindDirection10m { get; set; }
  [JsonProperty("wind_gusts_10m")] public double WindGusts10m { get; set; }
}
public class HourlyUnits
{
  public string? Time { get; set; }

  [JsonProperty("temperature_2m")]
  public string? Temperature2m { get; set; }

  [JsonProperty("precipitation_probability")]
  public string? PrecipitationProbability { get; set; }

  public string? Precipitation { get; set; }

  [JsonProperty("weather_code")]
  public string? WeatherCode { get; set; }

  [JsonProperty("wind_speed_10m")]
  public string? WindSpeed10m { get; set; }

  [JsonProperty("wind_direction_10m")]
  public string? WindDirection10m { get; set; }

  [JsonProperty("wind_gusts_10m")]
  public string? WindGusts10m { get; set; }
}
public class HourlyData
{
  public List<DateTime> Time { get; set; } = [];
  [JsonProperty("temperature_2m")] public List<double> Temperature2m { get; set; } = [];
  [JsonProperty("precipitation_probability")] public List<int> PrecipitationProbability { get; set; } = [];
  public List<double> Precipitation { get; set; } = [];
  [JsonProperty("weather_code")] public List<int> WeatherCode { get; set; } = [];
  [JsonProperty("wind_speed_10m")] public List<double> WindSpeed10m { get; set; } = [];
  [JsonProperty("wind_direction_10m")] public List<int> WindDirection10m { get; set; } = [];
  [JsonProperty("wind_gusts_10m")] public List<double> WindGusts10m { get; set; } = [];
}
public class DailyUnits
{
  public string? Time { get; set; }

  [JsonProperty("temperature_2m")]
  public string? Temperature2m { get; set; }

  [JsonProperty("precipitation_probability")]
  public string? PrecipitationProbability { get; set; }

  public string? Precipitation { get; set; }

  [JsonProperty("weather_code")]
  public string? WeatherCode { get; set; }

  [JsonProperty("wind_speed_10m")]
  public string? WindSpeed10m { get; set; }

  [JsonProperty("wind_direction_10m")]
  public string? WindDirection10m { get; set; }

  [JsonProperty("wind_gusts_10m")]
  public string? WindGusts10m { get; set; }
}
public class DailyData
{
  public List<DateTime> Time { get; set; } = [];
  public List<DateTime> Sunrise { get; set; } = [];
  public List<DateTime> Sunset { get; set; } = [];

  [JsonProperty("temperature_2m")]
  public List<double> Temperature2m { get; set; } = [];

  [JsonProperty("precipitation_probability")]
  public List<int> PrecipitationProbability { get; set; } = [];

  public List<double> Precipitation { get; set; } = [];

  [JsonProperty("weather_code")]
  public List<int> WeatherCode { get; set; } = [];

  [JsonProperty("wind_speed_10m")]
  public List<double> WindSpeed10m { get; set; } = [];

  [JsonProperty("wind_direction_10m")]
  public List<int> WindDirection10m { get; set; } = [];

  [JsonProperty("wind_gusts_10m")]
  public List<double> WindGusts10m { get; set; } = [];
}