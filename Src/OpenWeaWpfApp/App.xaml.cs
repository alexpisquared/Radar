#define Host_
namespace OpenWeaWpfApp;
public partial class App : Application
{
  string _audit = "Default!!";

#if Host
  readonly IHost _host;

  public App()
  {
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
    _ = services.AddTransient<PlotViewModel>();
    _ = services.AddSingleton<MainPlotViewWin>();
    _ = services.AddSingleton<MainPlotOldWindow>();
    _ = services.AddTransient<OpenWea>();
    _ = services.AddTransient<DbxHelper>();

    _ = services.AddSingleton<IConfigurationRoot>(AutoInitConfigHardcoded());

    _ = services.AddSingleton<ILogger>(sp => SeriLogHelper.InitLoggerFactory(
      folder: FSHelper.GetCreateSafeLogFolderAndFile(@$"C:\Temp\Logs\{Assembly.GetExecutingAssembly().GetName().Name![..5]}.{VersionHelper.Env()}.{Environment.UserName[..3]}..log"),
      levels: "+Info").CreateLogger<MainPlotViewWin>());

    Dbx(services);

    // need logging (ap: mar-12)

    _serviceProvider = services.BuildServiceProvider();
  }
#endif

  protected override async void OnStartup(StartupEventArgs e)
  {
    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
    EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox ?? new TextBox()).SelectAll(); }));
    ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

    SafeAudit();

    _serviceProvider.GetRequiredService<ILogger>().LogInformation($"║       ██  OnStrt  {_audit}");

#if !ParseToClasses

#if Host
    _host.Start();
    MainWindow = _host.Services.GetRequiredService<MainWindow>();                       // 1.050 ms!!!
#elif !OLD
    MainWindow = _serviceProvider.GetRequiredService<MainPlotViewWin>();                     //   400 ms
    MainWindow.DataContext = _serviceProvider.GetRequiredService<PlotViewModel>();      //   700 ms
#else
    MainWindow = _serviceProvider.GetRequiredService<MainPlotOldWindow>();                     //   400 ms
    MainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();      //   700 ms
#endif

    //the only way to populate PlotView: await ((MainViewModel)MainPlotOldWindow.DataContext).PopulateAll();

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
    await Task.Yield();
  }
  protected override void OnExit(ExitEventArgs e)
  {
    Current.DispatcherUnhandledException -= UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;

    base.OnExit(e);
  }

  void Dbx(IServiceCollection services) => _ = services.AddDbContext<WeatherxContext>(optionsBuilder => //tu: dbcontext connstr https://youtu.be/7OBMhoKieqk?t=505 + https://codedocu.com/details?d=2653&a=9&f=425&d=0  :Project\Manage Connected Svcs !!! 2021-12
  {
    try
    {
      var cfg = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets         

      //todo:
      //#if Host
      //    //_host.Start();
      //    var cf2 = _host.Services?.GetRequiredService<IConfigurationRoot>();
      //#else
      //    var cf2 = _serviceProvider?.GetRequiredService<IConfigurationRoot>();
      //#endif

      _ = optionsBuilder.UseSqlServer(cfg.GetConnectionString("Exprs") ?? throw new ArgumentNullException(nameof(services), "cfg.GetConnectionString('Exprs')"));
    }
    catch (Exception ex)
    {
      _serviceProvider.GetRequiredService<ILogger>().LogError(ex, _audit);
      Clipboard.SetText(ex.Message);
      _ = MessageBox.Show(ex.Message, "Exception - Clipboarded");
    }
  });
  void SafeAudit()
  {
    try
    {
      var cfg = _serviceProvider.GetRequiredService<IConfigurationRoot>();

      _audit = VersionHelper.DevDbgAudit(cfg, $"wai:{cfg["WhereAmI"]}");
    }
    catch (Exception ex)
    {
      _serviceProvider.GetRequiredService<ILogger>().LogError(ex, _audit);
    }
  }
  public static IConfigurationRoot AutoInitConfigHardcoded(string defaultValues = _defaultAppSetValues, bool enforceCreation = false)
  {
    var server = Environment.MachineName == "RAZER1" ? @".\SqlExpress" : "mtDEVsqldb,1625";
    var config = new ConfigurationBuilder()
      .AddInMemoryCollection()
      .Build();

    config["WhereAmI"] = "Mem";
    config["LogFolder"] = @"C:\temp\Logs\OWA..log"; 
    config["ServerList"] = @".\sqlexpress mtDEVsqldb,1625 mtUATsqldb mtPRDsqldb";
    config["SqlConStrSansSnI"] =   /**/  "Server={0};     Database={1};       persist security info=True;user id=IpmDevDbgUser;password=IpmDevDbgUser;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=152";
    config["SqlConStrSansSnD"] =   /**/  "Server={0};     Database={1};       Trusted_Connection=True;Encrypt=False;Connection Timeout=52";
    config["SqlConStrBR"] =        /**/ $"Server={server};Database=BR;        Trusted_Connection=True;Encrypt=False;Connection Timeout=52";
    config["SqlConStrVBCM"] =      /**/ $"Server={server};Database=VBCM;      Trusted_Connection=True;Encrypt=False;Connection Timeout=52";
    config["SqlConStrAlpha"] =     /**/ $"Server={server};Database=Alpha;     Trusted_Connection=True;Encrypt=False;Connection Timeout=52";
    config["SqlConStrBanking"] =   /**/ $"Server={server};Database=Banking;   Trusted_Connection=True;Encrypt=False;Connection Timeout=52";
    config["SqlConStrInventory"] = /**/ $"Server={server};Database=Inventory; Trusted_Connection=True;Encrypt=False;Connection Timeout=52";

#if !true
      var appConfig = new AppConfig();
      config.Bind(appConfig);
      string json = JsonConvert.SerializeObject(appConfig);
      File.WriteAllText("exported-" + _appSettingsFileNameOnly, json);
#endif

    return config;
  }
  const string
    _appSettingsFileNameOnly = "AppSettings.json",
    _defaultAppSetValues = @"{{
      ""WhereAmI"":             ""{0}"",
      ""LogFolder"":            ""\\\\bbsfile01\\Public\\Dev\\AlexPi\\Misc\\Logs\\..log"",
      ""ServerList"":           "".\\sqlexpress mtDEVsqldb,1625 mtUATsqldb mtPRDsqldb"",
      ""SqlConStrSansSnD"":     ""Server={{0}};Database={{1}};          Trusted_Connection=True;Encrypt=False;Connection Timeout=52"",
      ""SqlConStrBR"":          ""Server={{server}};Database=BR;        Trusted_Connection=True;Encrypt=False;Connection Timeout=52"",
      ""SqlConStrVBCM"":        ""Server={{server}};Database=VBCM;      Trusted_Connection=True;Encrypt=False;Connection Timeout=52"",
      ""SqlConStrAlpha"":       ""Server={{server}};Database=Alpha;     Trusted_Connection=True;Encrypt=False;Connection Timeout=52"",
      ""SqlConStrBanking"":     ""Server={{server}};Database=Banking;   Trusted_Connection=True;Encrypt=False;Connection Timeout=52"",
      ""SqlConStrInventory"":   ""Server={{server}};Database=Inventory; Trusted_Connection=True;Encrypt=False;Connection Timeout=52"",
      ""AppSettings"": {{
        ""ServerList"":         "".\\sqlexpress mtDEVsqldb mtUATsqldb mtPRDsqldb"",
        ""KeyVaultURL"":        ""<moved to a safer place>"",
        ""LastSaved"":          ""{2}""
      }}
}}";
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