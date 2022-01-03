using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Radar
{
  internal class OpenWeatherRevisit2022
  {
    public async Task<WeatherResponse?> Test()
    {
      const string city = "9ae59cac1cd2f6f64dd692d53cd118fc", ct = "ct";
      var url1 = $"https://api.openweathermap.org/data/2.5/forecast?id=524901&appid={city}";
      var url2 = $"https://api.openweathermap.org/data/2.5/weather?q={ct}&appid={city}";

      HttpClient _cf = new();

      var response = await _cf.GetAsync(url2);
      if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

      var weather = await response.Content.ReadFromJsonAsync<WeatherResponse>();

      return weather;
    }
  }

  internal class WeatherResponse
  {
  }
}
/*
 Example on how to make an API call using your city
API call

http://api.openweathermap.org/data/2.5/forecast?id=524901&appid={city}
Parameters
appid	required	Your unique city (you can always find it on your account page under the "city" tab)
Example of API call



 */