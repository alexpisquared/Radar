﻿using AmbienceLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMeteoClient.Infrastructure;
using OpenWeaWpfApp;
using StandardContractsLib;
using StandardLib.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenWeaApp;
public partial class App : Application
{
  string _audit = "Default!!";

  public IServiceProvider ServiceProvider { get; }

  public App()
  {
    ServiceCollection services = new();

    _ = services.AddSingleton<IBpr, Bpr>();
    _ = services.AddSingleton<IConfigurationRoot>(new ConfigurationBuilder().AddUserSecrets<App>().Build());
    _ = services.AddSingleton<ILogger>(sp => SeriLogHelper.CreateLogger<MainWeaWindow>());
    _ = services.AddSingleton<SpeechSynth>(sp => SpeechSynth.Factory(
      sp.GetRequiredService<IConfigurationRoot>()["AppSecrets:MagicSpeech"] ?? throw new ArgumentNullException(nameof(services), "cfg.AppSecrets:MagicSpeech"),
      sp.GetRequiredService<ILogger>()));

    _ = services.AddSingleton<MainWeaWindow>();

    AppStartHelper.InitOpenWeaServices(services);

    _ = services.AddOpenMeteoClient();

    ServiceProvider = services.BuildServiceProvider(); // LibInit.InitLib(services, _serviceProvider);
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
    EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox ?? new TextBox()).SelectAll(); }));
    ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

    _audit = VersionHelper.DevDbgAudit(ServiceProvider.GetRequiredService<IConfigurationRoot>(), "Hello");

    ServiceProvider.GetRequiredService<ILogger>().LogInformation($"OnStrt  {_audit}");

    MainWindow = ServiceProvider.GetRequiredService<MainWeaWindow>();
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