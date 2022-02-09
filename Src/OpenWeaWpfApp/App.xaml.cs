#define Host
namespace OpenWeaWpfApp;

public partial class App : Application
{
#if Host
  readonly IHost _host;

  public App()
  {
    Beep.Play();

    _host = Host.CreateDefaultBuilder()
      .AddViewModels()
      .ConfigureServices((hostContext, services) =>
      {
        _ = services.AddSingleton(s => new MainWindow()        {          DataContext = s.GetRequiredService<MainViewModel>()        });
        _ = services.AddTransient<OpenWea>();

        Dbx(services);

      })
      .Build();
  }
#else
  readonly IServiceProvider _serviceProvider;

  public App()
  {
    var services = new ServiceCollection();
    _ = services.AddTransient<MainViewModel>();
    _ = services.AddSingleton<MainWindow>(); 
        _ = services.AddTransient<OpenWea>();

    Dbx(services);

    _serviceProvider = services.BuildServiceProvider();
  }
#endif

  protected override async void OnStartup(StartupEventArgs e)
  {
#if !ParseToClasses

#if Host
    _host.Start();
    MainWindow = _host.Services.GetRequiredService<MainWindow>();                       // 1.050 ms!!!
#else
    MainWindow = _serviceProvider.GetRequiredService<MainWindow>();                     //   400 ms
    MainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();      //   700 ms
#endif

    //the only way to populate PlotView: await ((MainViewModel)MainWindow.DataContext).PopulateAsync();

    MainWindow.Show();
#else // Release:
    var config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    Trace.WriteLine($"---   WhereAmI: '{config["WhereAmI"]}'       {config["AppSecrets:MagicNumber"]}");

    /* //tu: not storing file in the GitHub:     (https://youtu.be/ASraHYMi808?t=832)
    git update-index    --assume-unchanged AppSettings.json
    git update-index --no-assume-unchanged AppSettings.json            */

    var rv = await new OpenWea().ParseJsonToClasses(config["AppSecrets:MagicNumber"]);
    Current.Shutdown();
#endif

    base.OnStartup(e);
    Write("");
    await Task.Yield();
  }


  void Dbx(IServiceCollection services) => _ = services.AddDbContext<WeatherxContext>(optionsBuilder => //tu: dbcontext connstr https://youtu.be/7OBMhoKieqk?t=505 + https://codedocu.com/details?d=2653&a=9&f=425&d=0  :Project\Manage Connected Svcs !!! 2021-12
  {
    var cfg = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets         
    //todo:
//#if Host
//    //_host.Start();
//    var cf2 = _host.Services?.GetRequiredService<IConfigurationRoot>();
//#else
//    var cf2 = _serviceProvider?.GetRequiredService<IConfigurationRoot>();
//#endif


    optionsBuilder.UseSqlServer(cfg.GetConnectionString("Exprs") ?? throw new ArgumentNullException(".GetConnectionString('Exprs')", nameof(services)));
  });
}

#if Host
public static class AddViewModelsHostBuilderExtensions
{
  public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
  {
    hostBuilder.ConfigureServices(services =>
    {
      services.AddSingleton<MainViewModel>();
    });

    return hostBuilder;
  }
}
#endif