namespace DB.WeatherX.PwrTls;

internal class Service0
{
  async Task UpdateDB()
  {
    var now = DateTime.Now;
    WeatherxContext dbx = new(null);

      var rv = dbx.PointReal.AnyAsync(d => d.SiteId == "");
      
  }

}
