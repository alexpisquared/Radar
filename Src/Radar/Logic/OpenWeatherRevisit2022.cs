using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Radar.OpenWeatherResponse;

namespace Radar.Logic;

internal class OpenWeatherRevisit2022
{
  public async Task<bool> Test(
    string code,
    double lat = -79.4829,
    double lon = 43.8001,
    string xtra = "&cnt=16",        // 16 is MAX.
    string city = "Concord,ON,CA",
    string time = "1586468027",
    string frmt = "json",           // XML gives readable time for sunrise!!!
    OpenWeatherCd what = OpenWeatherCd.CurrentWea)
  {
    try
    {
      var url = what switch
      {
        OpenWeatherCd.Forecast16 => $"https://api.openweathermap.org/data/2.5/forecast/daily?q={city}&units=metric&mode={frmt}{xtra}&appid={code}",  // https://openweathermap.org/forecast16          
        OpenWeatherCd.CurrentWea => $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&mode={frmt}&appid={code}",               // https://openweathermap.org/current
        OpenWeatherCd.TimeMachin => $"https://api.openweathermap.org/data/2.5/onecall/timemachine?lat={lat}&lon={lon}&dt={time}&appid={code}",       // https://openweathermap.org/api/one-call-api --5 days back only.
        /*    var url = 
      $"http://maps.openweathermap.org/maps/2.0/weather/TA2/{z}/{x}/{y}?date=1527811200&opacity=0.9&fill_bound=true&appid={code}";    <== needs $ subs-n ti seemes.
      $"https://pro.openweathermap.org/data/2.5/forecast/climate?q=London&appid={code}"; // https://openweathermap.org/api/forecast30 <== needs $ subs-n ti seemes.      */
        _ => "",
      };

      using var client = new HttpClient();
      var response = await client.GetAsync(url);
      if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

#if !true
    var json = await response.Content.ReadAsStringAsync();
    Trace.WriteLine(json);
    await File.WriteAllTextAsync($@"..\..\..\JsonResults\{city}-{xtra}-{what}-{DateTime.Now:yyMMdd·HHmmss}.{frmt}", json);
    var weather = "123";
#else
      object weather = what switch
      {
        OpenWeatherCd.Forecast16 => await response.Content.ReadFromJsonAsync<RootobjectForecast16>(), //todo: https://docs.microsoft.com/en-us/aspnet/core/web-api/route-to-code?view=aspnetcore-6.0
        OpenWeatherCd.CurrentWea => await response.Content.ReadFromJsonAsync<RootobjectCurrentWea>(),
        OpenWeatherCd.TimeMachin => throw new NotImplementedException()
      };
#endif

      return weather != null;
    }
    catch (Exception ex) { Trace.WriteLine(ex); throw; }
  }

  public enum OpenWeatherCd
  {
    Forecast16,
    CurrentWea,
    TimeMachin
  }
}