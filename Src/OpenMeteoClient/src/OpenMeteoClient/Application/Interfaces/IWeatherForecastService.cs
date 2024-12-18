using System.Threading.Tasks;
using OpenMeteoClient.Domain.Models;

namespace OpenMeteoClient.Application.Interfaces
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast?> GetForecastAsync(double latitude, double longitude);
    }
}