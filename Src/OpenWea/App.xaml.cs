namespace OpenWea;
public partial class App : Application
{
  string _audit = "Default!!";
  readonly IServiceProvider _serviceProvider;

  public App()
  {
    ServiceCollection services = new();
    //_ = services.AddTransient<MainViewModel>();
    //_ = services.AddTransient<PlotViewModel>();
    _ = services.AddSingleton<MainWeaWindow>();
    //_ = services.AddSingleton<MainPlotOldWindow>();
    //_ = services.AddTransient<OpenWea>();
    //_ = services.AddTransient<DbxHelper>();
    _ = services.AddSingleton<IBpr, Bpr>();

    _ = services.AddSingleton<IConfigurationRoot>(new ConfigurationBuilder().AddUserSecrets<App>().Build());

    _ = services.AddSingleton<ILogger>(sp => SeriLogHelper.InitLoggerFactory((@$"C:\Temp\Logs\{Assembly.GetExecutingAssembly().GetName().Name![..5]}.{Environment.UserName[..3]}..log"), "-Verbose -Info +Warning -Error -ErNT -11mb -Infi").CreateLogger<MainWeaWindow>());

    _ = services.AddSingleton<SpeechSynth>(sp => SpeechSynth.Factory(
      sp.GetRequiredService<IConfigurationRoot>()["AppSecrets:MagicSpeech"] ?? throw new ArgumentNullException("########"),
      sp.GetRequiredService<ILogger>()));

    _serviceProvider = services.BuildServiceProvider();
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
    EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox ?? new TextBox()).SelectAll(); }));
    ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

    _audit = VersionHelper.DevDbgAudit(_serviceProvider.GetRequiredService<IConfigurationRoot>(), "Hello");

    _serviceProvider.GetRequiredService<ILogger>().LogInformation($"OnStrt  {_audit}");

    MainWindow = _serviceProvider.GetRequiredService<MainWeaWindow>();
    MainWindow.Show();

    base.OnStartup(e);
  }
}
