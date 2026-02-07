using OpenMeteoClient.Infrastructure;
using OpenWeaSvc;
namespace OpenWeaWpfApp;
public static class AppStartHelper
{
  public static void InitOpenWeaServices(IServiceCollection services)
  {
    _ = services.AddTransient<PlotViewModel>();
    _ = services.AddTransient<OpenWea>();
    _ = services.AddTransient<DbxHelper>();
    _ = services.AddOpenMeteoClient();

    _ = services.AddDbContext<WeatherxContext>((serviceProvider, optionsBuilder) => //tu: dbcontext connstr https://youtu.be/7OBMhoKieqk?t=505 + https://codedocu.com/details?d=2653&a=9&f=425&d=0  :Project\Manage Connected Svcs !!! 2021-12
    {
      try
      {
        IConfigurationRoot cfg =  serviceProvider.GetRequiredService<IConfigurationRoot>(); // a better way (May 2024) than //(Application.Current as dynamic).ServiceProvider.GetService(typeof(IConfigurationRoot));   

        var connectionString = cfg.GetConnectionString("Exprs") ?? throw new ArgumentNullException(nameof(services), $"cfg.GetConnectionString('Exprs')\n\nCheck the app settings!!!\n\n{cfg["WhereAmI"]}\n\n");

        _ = optionsBuilder.UseSqlServer(connectionString);
      }
      catch (Exception ex) { ex.Pop(); }
    });

    ////from C:\g\PowerShellLog\Src\PowerShellLog\App.xaml.cs
    //_ = services.AddDbContext<A0DbContext>(optionsBuilder => //tu: dbcontext connstr https://youtu.be/7OBMhoKieqk?t=505 + https://codedocu.com/details?d=2653&a=9&f=425&d=0  :Project\Manage Connected Svcs !!! 2021-12
    //{
    //  var lgr = _serviceProvider?.GetRequiredService<ILogger<Window>>();
    //  var cfg = _serviceProvider?.GetRequiredService<IConfigurationRoot>();

    //  lgr?.LogInformation($"*** WhereAmI: {cfg?["WhereAmI"]}");

    //  optionsBuilder.UseSqlServer(cfg.GetConnectionString("LclDb") ?? throw new ArgumentNullException(".GetConnectionString('LclDb')"));
    //});
  }
}