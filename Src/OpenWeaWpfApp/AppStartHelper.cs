namespace OpenWeaWpfApp;

public static class AppStartHelper
{
  public static void InitOpenWeaServices(IServiceCollection services)
  {
    _ = services.AddTransient<PlotViewModel>();
    _ = services.AddTransient<OpenWea>();
    _ = services.AddTransient<DbxHelper>();

    _ = services.AddDbContext<WeatherxContext>(optionsBuilder => //tu: dbcontext connstr https://youtu.be/7OBMhoKieqk?t=505 + https://codedocu.com/details?d=2653&a=9&f=425&d=0  :Project\Manage Connected Svcs !!! 2021-12
    {
      try
      {
        IConfigurationRoot cfg = (Application.Current as dynamic).ServiceProvider.GetService(typeof(IConfigurationRoot)); 

        var scs = cfg.GetConnectionString("Exprs") ?? throw new ArgumentNullException(nameof(services), $"cfg.GetConnectionString('Exprs')\n\nCheck the app settings!!!\n\n{cfg["WhereAmI"]}\n\n");

        _ = optionsBuilder.UseSqlServer(scs);
      } 
      catch (Exception ex) { ex.Pop(); }
    });
  }
}