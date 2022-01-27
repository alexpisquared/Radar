#define Host_
namespace OpenWeaWpfApp;

public partial class App : Application
{
#if Host
  readonly IHost _host;

  public App()
  {
    _host = Host.CreateDefaultBuilder()
      .AddViewModels()
      .ConfigureServices((hostContext, services) =>
      {
        services.AddSingleton(s => new MainWindow()
        {
          DataContext = s.GetRequiredService<MainViewModel>()
        });
      })
      .Build();
  }
#else
  readonly IServiceProvider _serviceProvider;

  public App()
  {
    var services = new ServiceCollection();
    _ = services.AddTransient<MainViewModel>();
    _ = services.AddSingleton<MainWindow>(); // (sp => new MainView(sp.GetRequiredService<ILogger>(), sp.GetRequiredService<IConfigurationRoot>(), sp.GetRequiredService<InventoryContext>(), _started));
    _serviceProvider = services.BuildServiceProvider();
  }
#endif

  protected override async void OnStartup(StartupEventArgs e)
  {
#if DEBUG

#if Host
    _host.Start();
    MainWindow = _host.Services.GetRequiredService<MainWindow>();
#else
    MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
    MainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
#endif

    //the only way to populate PlotView: await ((MainViewModel)MainWindow.DataContext).PopulateAsync();

    MainWindow.Show();
#else // Release:
    var config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    Trace.WriteLine($"---   WhereAmI: '{config["WhereAmI"]}'       {config["AppSecrets:MagicNumber"]}");

    /* //tu: not storing file in the GitHub:     (https://youtu.be/ASraHYMi808?t=832)
    git update-index    --assume-unchanged AppSettings.json
    git update-index --no-assume-unchanged AppSettings.json            */

    var rv = await new OpenWeather2022.OpenWea().ParseJsonToClasses(config["AppSecrets:MagicNumber"]);
    Current.Shutdown();
#endif

    base.OnStartup(e); 
    Write("");
    await Task.Yield();
  }
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