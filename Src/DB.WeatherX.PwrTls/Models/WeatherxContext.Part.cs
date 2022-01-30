namespace DB.WeatherX.PwrTls.Models;

public partial class WeatherxContext
{
  public void EnsureExists()
  {
    if (Environment.UserDomainName != "RAZER1")
    {
      //Debugger.Break();
      Database.EnsureCreated(); // 693ms
    }
  }
}