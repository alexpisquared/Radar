using System.Diagnostics;

namespace DB.WeatherX.PwrTls.Models;

public partial class WeatherxContext
{
  public void EnsureCreated22()
  {
    try
    {
      Database.EnsureCreated(); // 693ms
    }
    catch (Exception ex)
    {
      Trace.WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@");
      if (Debugger.IsAttached) Debugger.Break(); else throw;
    }
  }
}