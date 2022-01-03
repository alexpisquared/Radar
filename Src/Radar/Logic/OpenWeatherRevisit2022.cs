using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Radar.Logic;

internal class OpenWeatherRevisit2022
{
  public async Task<WeatherResponse?> Test(
    string code = "07f9d460eb3cca612667ea5d3abbd81e", 
    string city = "Concord,ON,CA", 
    string mode = "xml")
  {
    double x = 1, y = 1, z = 1; // double x = -79.4829, y = 43.8001, z = 1;

    var url =      
      //$"https://api.openweathermap.org/data/2.5/forecast?id=524901&appid={code}";
      //$"http://maps.openweathermap.org/maps/2.0/weather/TA2/{z}/{x}/{y}?date=1527811200&opacity=0.9&fill_bound=true&appid={code}"; <== needs $ subs-n ti seemes.
      $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&mode={mode}&appid={code}"; // https://openweathermap.org/current

    HttpClient _cf = new();

    var response = await _cf.GetAsync(url);
    if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

    var weathex = await response.Content.ReadAsStringAsync();
    Trace.WriteLine(weathex);

    await File.WriteAllTextAsync($@"..\..\..\JsonResults\{city}-{DateTime.Now:yyMMdd·HHmmss}.{mode}", weathex);

    //var weather = await response.Content.ReadFromJsonAsync<WeatherResponse>();

    return new WeatherResponse();
  }
}

internal class WeatherResponse
{
}
