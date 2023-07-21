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
      try { _ = optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddUserSecrets<App>().Build()?.GetConnectionString("Exprs") ?? throw new ArgumentNullException(nameof(services), "cfg.GetConnectionString('Exprs')\n\nCheck the app settings!!!")); } //todo: bad: new ConfigurationBuilder().AddUserSecrets<App>().Build()
      catch (Exception ex) { _ = MessageBox.Show(ex.Message, "Exception in App()"); }
    });
  }
}