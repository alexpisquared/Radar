# OpenMeteoClient

OpenMeteoClient is a .NET library for fetching and processing weather forecast data from the Open-Meteo API. It follows Clean Architecture principles and implements best practices for API communication, error handling, and data modeling.

## Features

- Fetch weather forecast data from Open-Meteo API
- Caching of API responses
- Retry and circuit breaker policies for improved resilience
- Dependency injection for easy integration
- Logging support
- Debugging information for raw JSON and deserialized objects

## Installation

To use OpenMeteoClient in your project, add it as a reference to your .NET project.

## Usage

1. Add the OpenMeteoClient services to your dependency injection container:

```csharp
using OpenMeteoClient.Infrastructure;

public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddOpenMeteoClient();
    // ...
}
```

2. Inject and use the `IWeatherForecastService` in your application:

```csharp
using OpenMeteoClient.Application.Interfaces;
using OpenMeteoClient.Domain.Models;

public class WeatherController : ControllerBase
{
    private readonly IWeatherForecastService _weatherForecastService;

    public WeatherController(IWeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet]
    public async Task<ActionResult<WeatherForecast>> GetForecast(double latitude, double longitude)
    {
        var forecast = await _weatherForecastService.GetForecastAsync(latitude, longitude);
        return Ok(forecast);
    }
}
```

## Example Program

An example console application is included in the `Program.cs` file. This program demonstrates how to set up dependency injection and use the `WeatherForecastService` to fetch and display weather data for a specific location.

To run the example program:

1. Open a terminal in the project directory
2. Run the following command:

```
dotnet run --project src/OpenMeteoClient/OpenMeteoClient.csproj
```

This will display a weather forecast for Toronto, Canada (latitude 43.83, longitude -79.5), including the raw JSON response and the deserialized object.

## Debugging

The OpenMeteoApiClient includes debugging information that prints the raw JSON response and the re-serialized object to the console. This can be helpful for troubleshooting any issues with the API response or object deserialization.

## Running Tests

To run the unit tests, use the following command in the root directory of the project:

```
dotnet test
```

## Dependencies

- Newtonsoft.Json: Used for JSON serialization and deserialization
- Microsoft.Extensions.Caching.Memory: Used for in-memory caching
- Microsoft.Extensions.DependencyInjection: Used for dependency injection
- Microsoft.Extensions.Http: Used for HttpClient factory
- Microsoft.Extensions.Http.Polly: Used for retry and circuit breaker policies
- Microsoft.Extensions.Logging: Used for logging

## License

This project is licensed under the MIT License.