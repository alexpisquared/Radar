using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Radar.Logic;

internal class OpenWeatherRevisit2022
{
  public async Task<WeatherResponse?> Test(
    string code,
    string xtra = "&cnt=17", // 17 is MAX.
    string city = "Concord,ON,CA",
    string frmt = "xml") // XML gives readable time for sunrise!!!
  {
    //double x = 1, y = 1, z = 1; // double x = -79.4829, y = 43.8001, z = 1;

    var url = /*
      $"http://maps.openweathermap.org/maps/2.0/weather/TA2/{z}/{x}/{y}?date=1527811200&opacity=0.9&fill_bound=true&appid={code}";        <== needs $ subs-n ti seemes.
      $"https://pro.openweathermap.org/data/2.5/forecast/climate?q=London&appid={code}"; // https://openweathermap.org/api/forecast30 //  <== needs $ subs-n ti seemes.
      $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&mode={mode}&appid={code}";              // https://openweathermap.org/current */
      $"https://api.openweathermap.org/data/2.5/forecast/daily?q={city}&units=metric&mode={frmt}{xtra}&appid={code}"; // https://openweathermap.org/forecast16

    HttpClient _cf = new();

    var response = await _cf.GetAsync(url);
    if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

    var json = await response.Content.ReadAsStringAsync();
    Trace.WriteLine(json);

    await File.WriteAllTextAsync($@"..\..\..\JsonResults\{city}-{xtra}-{DateTime.Now:yyMMdd·HHmmss}.{frmt}", json);

    //var weather = await response.Content.ReadFromJsonAsync<WeatherResponse>();

    return new WeatherResponse();
  }
}

internal class WeatherResponse
{
}
/*
 * api.openweathermap.org/data/2.5/forecast/daily?q={city name}&cnt={cnt}&appid={API key}
 * api.openweathermap.org/data/2.5/weather?q={city name}&appid={API key}
 * https://pro.openweathermap.org/data/2.5/forecast/climate?q={city name},{country code}&appid={API key}
 * https://pro.openweathermap.org/data/2.5/forecast/climate?q=London&appid={API key}
 * https://pro.openweathermap.org/data/2.5/forecast/climate?lat=35&lon=139&appid={API key}
 */