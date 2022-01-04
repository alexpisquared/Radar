using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Radar.Logic.Forecast;

namespace Radar.Logic;

internal class OpenWeatherRevisit2022
{
  public async Task<Rootobject?> Test(
    string code,
    string xtra = "&cnt=17", // 17 is MAX.
    string city = "Concord,ON,CA",
    string time = "1586468027",
    string frmt = "json") // XML gives readable time for sunrise!!!
  {
    //double x = 1, y = 1, z = 1; //
    double lat = -79.4829, lon = 43.8001, z = 1;

    var url = /*
      $"http://maps.openweathermap.org/maps/2.0/weather/TA2/{z}/{x}/{y}?date=1527811200&opacity=0.9&fill_bound=true&appid={code}";        <== needs $ subs-n ti seemes.
      $"https://pro.openweathermap.org/data/2.5/forecast/climate?q=London&appid={code}"; // https://openweathermap.org/api/forecast30 //  <== needs $ subs-n ti seemes.
      $"https://api.openweathermap.org/data/2.5/onecall/timemachine?lat={lat}&lon={lon}&dt={time}&appid={code}";      // https://openweathermap.org/api/one-call-api --5 days back only.
      $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&mode={mode}&appid={code}";              // https://openweathermap.org/current       */
      $"https://api.openweathermap.org/data/2.5/forecast/daily?q={city}&units=metric&mode={frmt}{xtra}&appid={code}"; // https://openweathermap.org/forecast16      

    //http://api.openweathermap.org/data/2.5/onecall/timemachine?lat=60.99&lon=30.9&dt=1586468027&appid={API key}

    HttpClient _cf = new();
    var response = await _cf.GetAsync(url);
    if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

#if !true
    var json = await response.Content.ReadAsStringAsync();
    Trace.WriteLine(json);
    //await File.WriteAllTextAsync($@"..\..\..\JsonResults\{city}-{xtra}-{DateTime.Now:yyMMdd·HHmmss}.{frmt}", json);
#else
    object? weathe2 = await response.Content.ReadFromJsonAsync(typeof(object)) ;
    var weather = await response.Content.ReadFromJsonAsync<Rootobject>();
    return weather; // return new Rootobject();
#endif
  }
}