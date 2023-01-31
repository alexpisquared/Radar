﻿using AmbienceLib;
namespace OpenWeaWpfApp;
public partial class App : Application
{
    string _audit = "Default!!";
    readonly IServiceProvider _serviceProvider;

    public App()
    {
        ServiceCollection services = new();
        _ = services.AddTransient<MainViewModel>();
        _ = services.AddTransient<PlotViewModel>();
        _ = services.AddSingleton<MainPlotViewWin>();
        _ = services.AddSingleton<MainPlotOldWindow>();
        _ = services.AddTransient<OpenWea>();
        _ = services.AddTransient<DbxHelper>();

        _ = services.AddSingleton<IConfigurationRoot>(new ConfigurationBuilder()
                .AddUserSecrets<App>()
                .Build());

        _ = services.AddSingleton<ILogger>(sp => SeriLogHelper.InitLoggerFactory(
          folder: FSHelper.GetCreateSafeLogFolderAndFile(@$"C:\Temp\Logs\{Assembly.GetExecutingAssembly().GetName().Name![..5]}.{Environment.UserName[..3]}..log"),
          levels: "-Verbose -Info +Warning -Error -ErNT -11mb -Infi").CreateLogger<MainPlotViewWin>());

        _ = services.AddSingleton<SpeechSynth>(sp => SpeechSynth.Factory(sp.GetRequiredService<IConfigurationRoot>()["AppSecrets:MagicSpeech"] ?? throw new ArgumentNullException("########")));

        _ = services.AddDbContext<WeatherxContext>(optionsBuilder => //tu: dbcontext connstr https://youtu.be/7OBMhoKieqk?t=505 + https://codedocu.com/details?d=2653&a=9&f=425&d=0  :Project\Manage Connected Svcs !!! 2021-12
        {
            try
            {
                IConfigurationRoot? cfg = _serviceProvider?.GetRequiredService<IConfigurationRoot>();

                _ = optionsBuilder.UseSqlServer(cfg?.GetConnectionString("Exprs") ?? throw new ArgumentNullException(nameof(services), "cfg.GetConnectionString('Exprs')"));
            }
            catch (Exception ex)
            {
                _serviceProvider?.GetRequiredService<ILogger>().LogError(ex, _audit);
                Clipboard.SetText(ex.Message);
                _ = MessageBox.Show(ex.Message, "Exception - Clipboarded");
            }
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
        EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox ?? new TextBox()).SelectAll(); }));
        ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

        _audit = VersionHelper.DevDbgAudit(_serviceProvider.GetRequiredService<IConfigurationRoot>());

        _serviceProvider.GetRequiredService<ILogger>().LogInformation($"OnStrt  {_audit}");

#if !ParseToClasses
        MainWindow = _serviceProvider.GetRequiredService<MainPlotViewWin>();                //   400 ms
        MainWindow.DataContext = _serviceProvider.GetRequiredService<PlotViewModel>();      //   700 ms

        MainWindow.Show();
#else // ParseToClasses:
    var rv = await new OpenWea().ParseJsonToClasses(_serviceProvider.GetRequiredService .. config["AppSecrets:MagicNumber"]);
    Current.Shutdown();
#endif

        base.OnStartup(e);
    }
    protected override void OnExit(ExitEventArgs e)
    {
        Current.DispatcherUnhandledException -= UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;

        base.OnExit(e);
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