using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenMeteoClient.Application.Interfaces;
using OpenMeteoClient.Infrastructure;
using Pastel;
using System.Drawing;

Console.WriteLine("Hello, World!".Pastel(Color.FromArgb(65, 229, 0)));

// Set up dependency injection
var services = new ServiceCollection();
services.AddLogging(configure => configure.AddConsole());
services.AddOpenMeteoClient();
var serviceProvider = services.BuildServiceProvider();

var _openMeteoSvc = serviceProvider.GetRequiredService<IWeatherForecastService>();

try
{
  var forecast = await _openMeteoSvc.GetForecastAsync(43.83, -79.5);

  Console.WriteLine("Raw/Derivative JSON:".Pastel(Color.FromArgb(65, 229, 50)) );
  Console.WriteLine(JsonConvert.SerializeObject(forecast, Formatting.Indented).Pastel(Color.FromArgb(65, 129, 50)));

  Console.WriteLine("\nDeserialized object:");
  Console.WriteLine($"Weather forecast for coordinates: {forecast.Latitude}, {forecast.Longitude}");
  Console.WriteLine($"Time zone: {forecast.Timezone} ({forecast.TimezoneAbbreviation})");
  Console.WriteLine($"Elevation: {forecast.Elevation} meters");
      
  Console.WriteLine("\nHourly forecast:".Pastel(Color.FromArgb(65, 229, 50)));
  if (forecast.Hourly != null && forecast.Hourly.Time.Count > 0)
  {
    for (var i = 0; i < Math.Min(24, forecast.Hourly.Time.Count); i++)
    {
      Console.WriteLine($"{forecast.Hourly.Time[i]:yyyy-MM-dd HH:mm}  " +
          $"{forecast.Hourly.Temperature2m[i],4:N1}°C   ".Pastel(Color.FromArgb(65, 229, 250)) +
          $"Precipitation: {forecast.Hourly.Precipitation[i],3}mm   ".Pastel(Color.FromArgb(165, 29, 250)) +
          $"Wind Speed: {forecast.Hourly.WindSpeed10m[i],5:N1}km/h".Pastel(Color.FromArgb(165, 129, 250)));
    }
  }
}
catch (Exception ex)
{
  Console.WriteLine($"An error occurred: {ex.Message}".Pastel(Color.FromArgb(225, 129, 50)));
}
