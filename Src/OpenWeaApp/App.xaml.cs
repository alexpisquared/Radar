using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using AmbienceLib;
using DB.WeatherX.PwrTls.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenWeather2022;
using OpenWeaWpfApp;
using OpenWeaWpfApp.Helpers;
using StandardContractsLib;
using StandardLib.Helpers;

namespace OpenWeaApp;
public partial class App : Application
{
  string _audit = "Default!!";
  readonly IServiceProvider _serviceProvider;

  public App()
  {
    ServiceCollection services = new();
    //_ = services.AddTransient<MainPlotOldVM>();
    _ = services.AddTransient<PlotViewModel>();
    _ = services.AddSingleton<MainWeaWindow>();
    _ = services.AddSingleton<MainPlotOldWindow>();
    _ = services.AddTransient<OpenWea>();
    _ = services.AddTransient<DbxHelper>();
    _ = services.AddSingleton<IBpr, Bpr>();

    _ = services.AddSingleton<IConfigurationRoot>(new ConfigurationBuilder().AddUserSecrets<App>().Build());

    _ = services.AddSingleton<ILogger>(sp => SeriLogHelper.InitLoggerFactory((@$"C:\Temp\Logs\{Assembly.GetExecutingAssembly().GetName().Name![..5]}.{Environment.UserName[..3]}..log"), "-Verbose -Info +Warning -Error -ErNT -11mb -Infi").CreateLogger<MainWeaWindow>());

    _ = services.AddSingleton<SpeechSynth>(sp => SpeechSynth.Factory(
      sp.GetRequiredService<IConfigurationRoot>()["AppSecrets:MagicSpeech"] ?? throw new ArgumentNullException("########"),
      sp.GetRequiredService<ILogger>()));

    _ = services.AddDbContext<WeatherxContext>(optionsBuilder => //tu: dbcontext connstr https://youtu.be/7OBMhoKieqk?t=505 + https://codedocu.com/details?d=2653&a=9&f=425&d=0  :Project\Manage Connected Svcs !!! 2021-12
    {
      try
      {
        var cfg = _serviceProvider?.GetRequiredService<IConfigurationRoot>();

        _ = optionsBuilder.UseSqlServer(cfg?.GetConnectionString("Exprs") ?? throw new ArgumentNullException(nameof(services), "cfg.GetConnectionString('Exprs')"));
      }
      catch (Exception ex)
      {
        _serviceProvider?.GetRequiredService<ILogger>().LogError(ex, _audit);
        Clipboard.SetText(ex.Message);
        _ = MessageBox.Show(ex.Message, "Exception - Clipboarded");
      }
    });

    _serviceProvider = services.BuildServiceProvider(); // LibInit.InitLib(services, _serviceProvider);
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
    EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox ?? new TextBox()).SelectAll(); }));
    ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

    _audit = VersionHelper.DevDbgAudit(_serviceProvider.GetRequiredService<IConfigurationRoot>(), "Hello");

    _serviceProvider.GetRequiredService<ILogger>().LogInformation($"OnStrt  {_audit}");

    MainWindow = _serviceProvider.GetRequiredService<MainWeaWindow>();
    var DataContext_ = _serviceProvider.GetRequiredService<PlotViewModel>();
    MainWindow.DataContext = _serviceProvider.GetRequiredService<PlotViewModel>();
    MainWindow.Show();

    base.OnStartup(e);
  }
  protected override void OnExit(ExitEventArgs e)
  {
    Current.DispatcherUnhandledException -= UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;

    base.OnExit(e);
  }
}