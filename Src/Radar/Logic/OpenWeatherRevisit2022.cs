using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Radar.OpenWeatherResponse;

namespace Radar.Logic;

internal class OpenWeatherRevisit2022
{
  public async Task<bool> Test(string code)
  {
    Trace.Write($"{DateTimeToUnixTimestamp(DateTime.Today)} *******\n");

    //await Test_(code, what: OpenWeatherCd.Forecast16);
    //await Test_(code, what: OpenWeatherCd.CurrentWea);
    await Test_(code, what: OpenWeatherCd.TimeMachin, time: DateTimeToUnixTimestamp(DateTime.Today.AddDays(-5)).ToString()); // 0 - -5
    
    return true;
  }
  async Task<bool> Test_(
    string code,
    double lon = -79.4829,
    double lat = 43.8001,
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
        OpenWeatherCd.TimeMachin => $"https://api.openweathermap.org/data/2.5/onecall/timemachine?lat={lat}&units=metric&lon={lon}&dt={time}&appid={code}",       // openweathermap.org/api/one-call-api --5 days back only.
        OpenWeatherCd.Forecast16 => $"https://api.openweathermap.org/data/2.5/forecast/daily?q={city}&units=metric&mode={frmt}{xtra}&appid={code}",  // openweathermap.org/forecast16          
        OpenWeatherCd.CurrentWea => $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&mode={frmt}&appid={code}",               /* openweathermap.org/current        
        OpenWeatherCd.CurrentWea => $"http://maps.openweathermap.org/maps/2.0/weather/TA2/{z}/{x}/{y}?date=1527811200&opacity=0.9&fill_bound=true&appid={code}";    <== needs $ subs-n ti seemes.
        OpenWeatherCd.CurrentWea => $"https://pro.openweathermap.org/data/2.5/forecast/climate?q=London&appid={code}"; // https://openweathermap.org/api/forecast30 <== needs $ subs-n ti seemes.      */
        _ => "",
      };

      using var client = new HttpClient();
      var response = await client.GetAsync(url);
      if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

#if true
    var json = await response.Content.ReadAsStringAsync();
    Trace.WriteLine(json);
    await System.IO.File.WriteAllTextAsync($@"..\..\..\JsonResults\{city}-{xtra}-{what}-{DateTime.Now:yyMMdd·HHmmss}.{frmt}", json);
#else
      switch (what) //todo: https://docs.microsoft.com/en-us/aspnet/core/web-api/route-to-code?view=aspnetcore-6.; break ;
      {
        case OpenWeatherCd.Forecast16: var f16 = await response.Content.ReadFromJsonAsync<RootobjectForecast16>(); f16?.list.ToList().ForEach(x => Trace.WriteLine($":> {x.sunrise}  {x}")); break;
        case OpenWeatherCd.CurrentWea: var pr5 = await response.Content.ReadFromJsonAsync<RootobjectCurrentWea>(); pr5?.weather.ToList().ForEach(x => Trace.WriteLine($":> {x}")); break;
        case OpenWeatherCd.TimeMachin: var tmn = await response.Content.ReadFromJsonAsync<RootobjectTimeMachin>(); tmn?.hourly.ToList().ForEach(x => Trace.WriteLine($":> {x}")); break;
        default: throw new NotImplementedException();
      }
#endif

      return url != null;
    }
    catch (Exception ex) { Trace.WriteLine(ex); throw; }
  }

  public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
  {
    // Unix timestamp is seconds past epoch
    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    return dateTime;
  }
  public static double DateTimeToUnixTimestamp(DateTime dateTime)
  {
    return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -           new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
  }

  public enum OpenWeatherCd
  {
    Forecast16,
    CurrentWea,
    TimeMachin
  }
}