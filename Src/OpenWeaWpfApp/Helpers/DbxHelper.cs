namespace OpenWeaWpfApp.Helpers;
public class DbxHelper
{
  public DbxHelper(WeatherxContext weatherxContext)
  {
    WeatherxContext = weatherxContext;
  }

  public WeatherxContext WeatherxContext { get; }
}
