using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DB.WeatherX.PwrTls;

internal class Service0
{
  async Task UpdateDB(string connectionString)
  {
    var now = DateTime.Now;

    WeatherxContextFactory dbf = new(connectionString);
    using WeatherxContext dbx = dbf.CreateDbContext();

    var rv = dbx.PointReal.AnyAsync(d => d.SiteId == "");
  }
}

public class WeatherxContextFactory
{
  readonly string _connectionString;

  public WeatherxContextFactory(string connectionString) => _connectionString = connectionString;

  public WeatherxContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<WeatherxContext>().UseSqlServer(_connectionString).Options;

    return new WeatherxContext(options);
  }
}

public class ReservoomDesignTimeDbContextFactory : IDesignTimeDbContextFactory<WeatherxContext>
{
  public WeatherxContext CreateDbContext(string[] args)
  {
    var options = new DbContextOptionsBuilder<WeatherxContext>().UseSqlServer("Data Source=reservoom.db").Options;

    return new WeatherxContext(options);
  }
}

