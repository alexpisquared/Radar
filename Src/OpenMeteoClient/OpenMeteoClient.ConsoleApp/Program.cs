using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenMeteoClient.Application.Interfaces;
using OpenMeteoClient.Infrastructure;
// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OpenMeteoClient.Application.Interfaces;
using Pastel;
using System.Drawing;

Console.WriteLine("Hello, World!".Pastel(Color.FromArgb(165, 229, 250)));


  // Set up dependency injection
  var services = new ServiceCollection();
  services.AddLogging(configure => configure.AddConsole());
  services.AddOpenMeteoClient();

  var serviceProvider = services.BuildServiceProvider();

  // Get an instance of IWeatherForecastService
  var weatherForecastService = serviceProvider.GetRequiredService<IWeatherForecastService>();

  try
  {
    // Fetch weather forecast for Toronto, Canada
    var forecast = await weatherForecastService.GetForecastAsync(43.83, -79.5);

    Console.WriteLine("Raw JSON:");
    Console.WriteLine(JsonConvert.SerializeObject(forecast, Formatting.Indented));

    Console.WriteLine("\nDeserialized object:");
    Console.WriteLine($"Weather forecast for coordinates: {forecast.Latitude}, {forecast.Longitude}");
    Console.WriteLine($"Time zone: {forecast.Timezone} ({forecast.TimezoneAbbreviation})");
    Console.WriteLine($"Elevation: {forecast.Elevation} meters");

    if (forecast.Hourly != null && forecast.Hourly.Time.Count > 0)
    {
      Console.WriteLine("\nHourly forecast:");
      for (int i = 0; i < Math.Min(24, forecast.Hourly.Time.Count); i++)
      {
        Console.WriteLine($"{forecast.Hourly.Time[i]:yyyy-MM-dd HH:mm}  " +
            $"{forecast.Hourly.Temperature2m[i],4:N1}°C   " +
            $"Precipitation: {forecast.Hourly.Precipitation[i],3}mm   " +
            $"Wind Speed: {forecast.Hourly.WindSpeed10m[i],5:N1}km/h".Pastel(Color.FromArgb(165, 129, 250)));
      }
    }
  }
  catch (Exception ex)
  {
    Console.WriteLine($"An error occurred: {ex.Message}");
  }
