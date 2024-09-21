using System.Threading.Tasks;
using OpenMeteoClient.Domain.Models;

namespace OpenMeteoClient.Infrastructure.Interfaces
{
    public interface IOpenMeteoApiClient
    {
        Task<WeatherForecast> GetForecastAsync(double latitude, double longitude);
    }
}