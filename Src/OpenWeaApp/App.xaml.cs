using System;
using System.IO;
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
  readonly IServiceProvider _serviceProvider; public IServiceProvider ServiceProvider => _serviceProvider;

  public App()
  {
    ServiceCollection services = new();

    _ = services.AddSingleton<IBpr, Bpr>();
    _ = services.AddSingleton<IConfigurationRoot>(new ConfigurationBuilder().AddUserSecrets<App>().Build());
    _ = services.AddSingleton<ILogger>(sp => SeriLogHelper.InitLoggerFactory((@$"{Path.Combine(OneDrive.Root, @"Public")}\Logs\{Assembly.GetExecutingAssembly().GetName().Name![..5]}.{Environment.UserName[..3]}..log"), "-Verbose -Info -Warning -Error -ErNT -11mb +Infi").CreateLogger<MainWeaWindow>());
    _ = services.AddSingleton<SpeechSynth>(sp => SpeechSynth.Factory(
      sp.GetRequiredService<IConfigurationRoot>()["AppSecrets:MagicSpeech"] ?? throw new ArgumentNullException(nameof(services), "cfg.AppSecrets:MagicSpeech"),
      sp.GetRequiredService<ILogger>()));

    _ = services.AddSingleton<MainWeaWindow>();

    AppStartHelper.InitOpenWeaServices(services);

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
    //use ServiceProvider instead: MainWindow.DataContext = _serviceProvider.GetRequiredService<PlotViewModel>();
    MainWindow.Show();

    base.OnStartup(e);
  }
  protected override void OnExit(ExitEventArgs e)
  {
    Current.DispatcherUnhandledException -= UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;

    base.OnExit(e);
  }
}