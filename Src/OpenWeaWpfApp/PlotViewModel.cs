#define ObsCol // Go figure: ObsCol works, while array NOT! Just an interesting factoid.
using OpenMeteoClient.Application.Interfaces;
using OpenMeteoClient.Domain.Models;
using OpenWeaSvc;

namespace OpenWeaWpfApp;

public partial class PlotViewModel : ObservableValidator
{
  #region fields
  readonly DateTime _startedAt = DateTime.Now;
  readonly Bpr bpr = new();
  readonly int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600;
  readonly IConfigurationRoot _cfg;

  readonly OpenWea _opnwea;
  readonly IWeatherForecastService _openMeteoSvc;
  readonly DbxHelper _dbh;
  readonly ILogger _lgr;
  readonly bool _store;
  const int _maxIcons = 50;
  double _extrMax = +35, _extrMin = +05;

  //ImageSource _i; public ImageSource WeaIcom { get => _i; set => SetProperty(ref _i, value); }
  //Uri _k = new("http://openweathermap.org/img/wn/04n@2x.png"); public Uri WIcon { get => _k; set => SetProperty(ref _k, value); }
  const double _wk = 10f;
  const double _ms2kh = 3.6f * _wk;
  readonly ObservableCollection<ScatterPoint> SctrPtTPFVgn = [];
  //readonly ObservableCollection<ScatterPoint> SctrPtTPFMis = [];
  readonly ObservableCollection<ScatterPoint> SctrPtTPFOMt = [];

  readonly ObservableCollection<DataPoint> OwaLoclTemp = [];
  readonly ObservableCollection<DataPoint> OwaLoclFeel = [];
  readonly ObservableCollection<DataPoint> OwaLoclGust = []; // not shown :tmi
  readonly ObservableCollection<DataPoint> OwaTempExtr = [];
  readonly ObservableCollection<DataPoint> OwaLoclPopr = [];

  readonly ObservableCollection<DataPoint> OMtLoclTemp = [];
  readonly ObservableCollection<DataPoint> OMtLoclFeel = []; //
  readonly ObservableCollection<DataPoint> OMtLoclPrsr = []; //
  readonly ObservableCollection<DataPoint> OMtLoclGust = [];
  readonly ObservableCollection<DataPoint> OMtTempExtr = []; //
  readonly ObservableCollection<DataPoint> OMtLoclPopr = [];

  readonly ObservableCollection<DataPoint> Sunrise_Set = [];
  readonly ObservableCollection<DataPoint> SunSinusoid = [];
  readonly ObservableCollection<DataPoint> OMtLoclWind = [];
  readonly ObservableCollection<DataPoint> ECaToroTemp = [];
  readonly ObservableCollection<DataPoint> ECaVghnTemp = [];
  readonly ObservableCollection<DataPoint> ECaMrkhTemp = [];
  readonly ObservableCollection<DataPoint> ECaMissTemp = [];
  readonly ObservableCollection<DataPoint> ECaTIslTemp = [];

  [Browsable(false)][ObservableProperty] PlotModel model = new() { TextColor = OxyColors.Lavender }; // Title on the top of the plot.
  [ObservableProperty] double timeMin = DateTime.Today.ToOADate() - 1; partial void OnTimeMinChanged(double value) => ReCreateAxises("T min");
  [ObservableProperty] double timeMax = DateTime.Today.ToOADate() + 2; partial void OnTimeMaxChanged(double value) => ReCreateAxises("T max");
  [ObservableProperty] string plotTitle = "";
  [ObservableProperty] string currentConditions = "";
  [ObservableProperty] string curTempReal = "";
  [ObservableProperty] string curTempFeel = "";
  [ObservableProperty] string curWindKmHr = "";
  [ObservableProperty] string curSrcColor = "#00f";
  [ObservableProperty] int windDirn;
  [ObservableProperty] double windVeloKmHr;
  [ObservableProperty] ObservableCollection<string> opnWeaIcoA = [];
#if ObsCol
  [ObservableProperty] ObservableCollection<ImageSource> opnWeaIco3 = [];
  [ObservableProperty] ObservableCollection<string> opnWeaTip3 = [];
#else
  ImageSource[] _3 = new ImageSource[_maxIcons]; public ImageSource[] OpnWeaIco3 { get => _3; set => SetProperty(ref _3, value); }
  string[] _4 = new string[_maxIcons]; public string[] OpnWeaTip3 { get => _4; set => SetProperty(ref _4, value); }
  //Messages _m3 = new(); public Messages OpnWeaIco3 { get => _m3; set => SetProperty(ref _m3, value); }
  //Messages _m4 = new(); public Messages OpnWeaTip3 { get => _m4; set => SetProperty(ref _m4, value); }
#endif
  #endregion

  public PlotViewModel(OpenWea openWea, IWeatherForecastService openMeteo, DbxHelper dbh, ILogger lgr, IConfigurationRoot cfg)
  {
    _cfg = cfg; // _cfg = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    _dbh = dbh;
    _lgr = lgr;
    _opnwea = openWea;
    _openMeteoSvc = openMeteo;
    _store = _cfg["StoreDataToDB"] == "Yes";

    for (var i = 0; i < _maxIcons; i++)
    {
#if ObsCol // must add for the binding to have something to hook on to for OpnWea[xx].
      OpnWeaIco3.Add(new BitmapImage(new Uri("http://openweathermap.org/img/wn/01n.png")));
      OpnWeaTip3.Add("Optimistic Init");
#else
      OpnWeaIco3[i]=(new BitmapImage(new Uri("http://openweathermap.org/img/wn/01d.png")));
      OpnWeaTip3[i]=("Optimistic Init");
#endif
    }

    ModelClearAdd("ctor");
    _lgr.Log(LogLevel.Trace, "PlotViewModel() EOCtor");

    //if (VersionHelper.IsDbg) _synth.SpeakFAF("Are you sure?", voice: "en-US-AriaNeural", style: "terrified", speakingRate: .25, volumePercent: 75);
    //if (VersionHelper.IsDbg) _synth.SpeakFAF("Done!");
  }

  [ObservableProperty] string lastBuild = VersionHelper.CurVerStr;
  [ObservableProperty] string titleM = "";
  [ObservableProperty] string titleV = "";
  [ObservableProperty] string titleO = "";

  [RelayCommand]
  public async Task PopulateAll(object? obj)
  {
    if (obj is null) bpr.Start();

    try { await PopulateScatModel(obj); } catch (Exception ex) { ex.Pop(_lgr); }

    try { await PrevForecastFromDb(obj); } catch (Exception ex) { ex.Pop(_lgr); }      //_lgr.Log(LogLevel.Error, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
  }
  [RelayCommand]
  async Task PrevForecastFromDb(object? obj)
  {
    if (obj is null) bpr.Click();
    if (!_store) return;
    try
    {
      (List<PointFore>? vgn, List<PointFore>? mis, List<PointFore>? omt, TimeSpan timeTook) = await GetPastForecastFromDB();

      //tmi: mis.ForEach(r => SctrPtTPFMis.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
      vgn.ForEach(r => SctrPtTPFVgn.Add(CreateScatterPoint(r, "vgn")));
      omt.ForEach(r => SctrPtTPFOMt.Add(CreateScatterPoint(r, "omt")));

      Model.InvalidatePlot(true);
      SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  Populated: From DB \ttook {timeTook.TotalSeconds:N1} s   \n");
      //bpr.Tick();
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }
  }

  static ScatterPoint CreateScatterPoint(PointFore r, string v)
  {
    WriteLine($"   {v}:    {r.ForecastedFor.DateTime} \t {DateTimeAxis.ToDouble(r.ForecastedFor.DateTime):N2} \t size: {3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours),4:N1} \t y: {r.MeasureValue,4:N1} \t {(r.ForecastedFor - r.ForecastedAt).TotalHours,5:N1}h ");
    return new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h");
  }
  string SetSiteCurrentConditions(currentConditionsType? cc, dateStampType[]? dt, string siteNick)
  {
    CurTempReal = $"{float.Parse(cc?.temperature?.Value ?? "-888"):+##.#;-##.#;0}°";
    CurTempFeel = $"{float.Parse(cc?.windChill?.Value ?? cc?.temperature?.Value ?? "-888"):+##;-##;0}°";
    CurWindKmHr = $"{cc?.wind?.speed?.Value} {cc?.wind?.speed.units}";
    CurSrcColor = siteNick switch { "Miss" => "#080", "Vaug" => "#08f", _ => "#dd0" };

    return dt is { Length: > 1 } ? $"{siteNick} {dt[1].hour.Value}:{dt[1].minute}:  {CurTempReal}  +  {CurWindKmHr}  =  {CurTempFeel}" : $"{siteNick} .. is NUL";
  }
  [RelayCommand]
  async Task PopulateScatModel(object? obj)
  {
    _ = Task.Run(async () => await PlotViewModelHelpers.GetPast24hrFromEC(Cnst._Past24YYZ)).ContinueWith(_ => { DrawPast24hrEC(Cnst.pearson, _.Result); _pastPea = _.Result; }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetPast24hrFromEC(Cnst._Past24OKN)).ContinueWith(_ => { DrawPast24hrEC(Cnst.kingCty, _.Result); _pastKng = _.Result; }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._mississ)).ContinueWith(_ => { DrawFore24hrEC(Cnst._mississ, _.Result); _foreMis = _.Result; TitleM = SetSiteCurrentConditions(_.Result?.currentConditions, _.Result?.currentConditions?.dateTime, "Miss"); }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._richmhl)).ContinueWith(_ => { DrawFore24hrEC(Cnst._richmhl, _.Result); _foreVgn = _.Result; TitleV = SetSiteCurrentConditions(_.Result?.currentConditions, _.Result?.currentConditions?.dateTime, "Vaug"); }, TaskScheduler.FromCurrentSynchronizationContext());    //_ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._toronto)).ContinueWith(_ => { DrawFore24hrEC(Cnst._toronto, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());    //_ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._torIsld)).ContinueWith(_ => { DrawFore24hrEC(Cnst._torIsld, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());    //_ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._markham)).ContinueWith(_ => { DrawFore24hrEC(Cnst._markham, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());

    try
    {
      _openMeteo = await _openMeteoSvc.GetForecastAsync(43.83, -79.5);

      ArgumentNullException.ThrowIfNull(_openMeteo);
      ArgumentNullException.ThrowIfNull(_openMeteo.Current);
      ArgumentNullException.ThrowIfNull(_openMeteo.Daily);

      TitleO = $"OMt {_openMeteo.Current.Time:HH:mm}: {_openMeteo.Current.Temperature2m:+##.#;-##.#;0}°  +  {_openMeteo.Current.WindSpeed10m /** _ms2kh / _wk*/:N0} k/h  =  {_openMeteo.Current.ApparentTemperature:+##;-##;0}°";
      WindDirn = _openMeteo.Current.WindDirection10m;
      WindVeloKmHr = _openMeteo.Current.WindSpeed10m; //  * _ms2kh / _wk;
      WindGustKmHr = _openMeteo.Current.WindGusts10m; //  * _ms2kh / _wk;
                                                      //CurTempReal = $"{_openMeteo.Current.Temperature2m:+#.#;-#.#;0}°";
                                                      //CurTempFeel = $"{_openMeteo.Current.ApparentTemperature:+#;-#;0}°";
                                                      //CurWindKmHr = $"{WindVeloKmHr:N1}";

      for (var i = 0; i < _openMeteo.Hourly?.Time.Count; i++)
      {
        var t = _openMeteo.Hourly.Time[i].ToOADate();

        //OMtLoclFeel.Add(new DataPoint(t, _openMeteo.Hourly.app));
        OMtLoclTemp.Add(new DataPoint(t, _openMeteo.Hourly.Temperature2m[i]));
        OMtLoclPrsr.Add(new DataPoint(t, _openMeteo.Hourly.Pressure[i] - 1000));
        OMtLoclGust.Add(new DataPoint(t, _openMeteo.Hourly.WindGusts10m[i] * 10));
        OMtLoclPopr.Add(new DataPoint(t, _openMeteo.Hourly.PrecipitationProbability[i]));

        var rad = Math.PI * _openMeteo.Hourly.WindDirection10m[i] * 2 / 360;
        var dx = 0.10 * Math.Cos(rad);
        var dy = 10.0 * Math.Sin(rad);
        var tx = .002 * Math.Cos(rad + 90);
        var ty = 0.20 * Math.Sin(rad + 90);
        var sx = .002 * Math.Cos(rad - 90);
        var sy = 0.20 * Math.Sin(rad - 90);
        OMtLoclWind.Add(new DataPoint(t + tx, ty + (_openMeteo.Hourly.WindSpeed10m[i] * _wk)));
        OMtLoclWind.Add(new DataPoint(t + dx, dy + (_openMeteo.Hourly.WindSpeed10m[i] * _wk)));
        OMtLoclWind.Add(new DataPoint(t + sx, sy + (_openMeteo.Hourly.WindSpeed10m[i] * _wk)));
      }

      for (var i = 0; i < _openMeteo.Daily?.Sunrise.Count; i++)
      {
        var sunR = _openMeteo.Daily.Sunrise[i].ToOADate();
        var sunS = _openMeteo.Daily.Sunset[i].ToOADate();
        Sunrise_Set.Add(new DataPoint(sunR, -000));
        Sunrise_Set.Add(new DataPoint(sunR, +800));
        Sunrise_Set.Add(new DataPoint(sunS, +800));
        Sunrise_Set.Add(new DataPoint(sunS, -000));
      }

      ArgumentNullException.ThrowIfNull(_openMeteo.Daily);
      DrawSunSinusoid(_openMeteo.Daily.Sunrise.First().ToOADate()); // DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 7.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 6.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 5.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 4.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 3.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);       DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 2.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 1.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 0.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 1.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 2.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 3.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 4.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 5.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 6.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 7.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        */      }
    }
    catch (Exception ex) { ex.Pop(_lgr); }

    await GetDays(7); // 8 is too much: last day is empty of data (2024-12)

    Model.InvalidatePlot(true);
    SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  OMt  \n");

    //await DelayedStoreToDbIf(); // ERR: A second operation was started on this context instance before a previous operation completed. 

    _ = await DelayedStoreToDbIf();
  }

  async Task<(List<PointFore> vgn, List<PointFore> mis, List<PointFore> omt, TimeSpan timeTook)> GetPastForecastFromDB()
  {
    var startedAt = Stopwatch.GetTimestamp();
    while (_isDbBusy)
    {
      await bpr.WarnAsync(); // not quite a full solution
    }

    _isDbBusy = true;
    try
    {
      DateTime now = DateTime.Now;
      DateTime ytd = now.AddHours(-24);
      DateTime dby = now.AddHours(-48); // forecast for the past 24 hours is was done in the past 48 hours ... kind of.
      WeatherxContext dbx = _dbh.WeatherxContext;

      //if (Environment.UserDomainName != "RAZER1") try { _dbx.EnsureCreated22(); } catch (Exception ex) { _lgr.Log(LogLevel.Trace, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }
      //_lgr.Log(LogLevel.Trace, $"■97 {dbx.Database.GetConnectionString()}"); // 480ms

      List<PointFore> vgn = await dbx.PointFore.Where(r => r.SrcId == "eca" && r.SiteId == Cnst._vgn && r.ForecastedAt < r.ForecastedFor && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).OrderBy(r => r.ForecastedFor).ToListAsync();
      List<PointFore> mis = await dbx.PointFore.Where(r => r.SrcId == "eca" && r.SiteId == Cnst._mis && r.ForecastedAt < r.ForecastedFor && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).OrderBy(r => r.ForecastedFor).ToListAsync();
      List<PointFore> omt = await dbx.PointFore.Where(r => r.SrcId == "omt" && r.SiteId == Cnst._phc && r.ForecastedAt < r.ForecastedFor && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).OrderBy(r => r.ForecastedFor).ToListAsync();

      if (Debugger.IsAttached)
      {
        IOrderedQueryable<PointFore> sql = dbx.PointFore.Where(r => r.SrcId == "eca" && r.SiteId == Cnst._vgn && r.ForecastedAt < r.ForecastedFor && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).OrderBy(r => r.ForecastedFor);
        WriteLine(((Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable<DB.WeatherX.PwrTls.Models.PointFore>)sql).DebugView.Query);
      }

      return (vgn, mis, omt, Stopwatch.GetElapsedTime(startedAt));
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■88 {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

      return (new List<PointFore>(), new List<PointFore>(), new List<PointFore>(), Stopwatch.GetElapsedTime(startedAt)); // ..or throw;
    }
    finally { _isDbBusy = false; }
  }
  void DrawTemperatureExtremesSetBoundsForAxisY()
  {
#if ResaveToHardcodedField // Jun 2024
    using var fileStream = new FileStream(@"C:\g\Radar\Src\OpenWeaWpfApp\weather.gc.ca\en_climate_almanac_ON_6158733.xml", FileMode.Open);
    var almanacAll = ((climatedata?)new XmlSerializer(typeof(climatedata)).Deserialize(fileStream));
    Clipboard.SetText(JsonStringSerializer.Save(almanacAll)); // paste to ClimatedataStore.Json
#else
    ClimateData? almanacAll = JsonStringSerializer.Load<ClimateData>(ClimatedataStore.Json ?? "");
#endif

    climatedataMonthDay? almanac = almanacAll?.month[DateTime.Today.Month - 1].day[DateTime.Today.Day - 1]; // ~50 ms

    ArgumentNullException.ThrowIfNull(almanac, $"@1232!!!!!!!!!!!!!!!~~~~~~~~~");

    _extrMax = (double)almanac.temperature[0].Value;
    _extrMin = (double)almanac.temperature[1].Value;
    NormTMax = (double)almanac.temperature[2].Value;
    NormTMin = (double)almanac.temperature[3].Value;

    const int pad = 2;
    YAxisMin = (Math.Floor((_extrMax - 50) / 10) * 10) - pad;
    YAxisMax = _extrMax + 5;

    YAxsRMin = -10 * pad;
    YAxsRMax = (YAxisMax - YAxisMin - pad) * 10;

    DateTime now = DateTime.Now;
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now.AddDays(-2)), _extrMax));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now.AddDays(+8)), _extrMax));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now.AddDays(+8)), NormTMax));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now), NormTMax));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now), +100));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now), -100));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now), NormTMin));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now.AddDays(+8)), NormTMin));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now.AddDays(+8)), _extrMin));
    OwaTempExtr.Add(new DataPoint(DateTimeAxis.ToDouble(now.AddDays(-2)), _extrMin));
  }
  async Task<bool> DelayedStoreToDbIf()
  {
    if (!_store || VersionHelper.IsDbg)
    {
      SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  Store to DB not supported in Debug mode ...or on this PC \n");
      return false;
    }

    await Task.Delay(120_000);

    var minimumTimeToStoreInHr = 6;

    DateTimeOffset lastDbStoreTime = await PlotViewModelHelpers.LastTimeStoredToDb(_lgr, _dbh.WeatherxContext);
    TimeSpan timeSinceLastDbStore = DateTimeOffset.Now - lastDbStoreTime;
    if (timeSinceLastDbStore.TotalHours < minimumTimeToStoreInHr)
    {
      SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  Store to DB postponed till after {lastDbStoreTime.AddHours(minimumTimeToStoreInHr):H:mm}. \n");
      return false;
    }

    try
    {
      await PlotViewModelHelpers.AddPast24hrToDB_EnvtCa(_lgr, _dbh.WeatherxContext, Cnst.pearson, _pastPea);
      await PlotViewModelHelpers.AddPast24hrToDB_EnvtCa(_lgr, _dbh.WeatherxContext, Cnst.kingCty, _pastKng);

      await PlotViewModelHelpers.AddForecastToDB_EnvtCa(_lgr, _dbh.WeatherxContext, Cnst._mis, _foreMis);
      await PlotViewModelHelpers.AddForecastToDB_EnvtCa(_lgr, _dbh.WeatherxContext, Cnst._vgn, _foreVgn);
      await PlotViewModelHelpers.AddForecastToDB_OpnMto(_lgr, _dbh.WeatherxContext, Cnst._phc, _openMeteo);

      SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  All stored to DB after {timeSinceLastDbStore.TotalHours:N1} hr \n");

      return true;
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr);
      return false;
    }
  }
  void DrawPast24hrEC(string site, List<MeteoDataMy> lst)
  {
    try
    {
      switch (site)
      {
        //tmi:
        case Cnst.pearson: ECaMissTemp.Clear(); lst.OrderBy(x => x.TakenAt).ToList().ForEach(x => ECaMissTemp.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), x.TempAir))); break;
        case Cnst.kingCty: ECaVghnTemp.Clear(); lst.OrderBy(x => x.TakenAt).ToList().ForEach(x => ECaVghnTemp.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), x.TempAir))); break;
        default: break;
      }

      Model.InvalidatePlot(true); SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  Envt CA  Past       \t{site}\t {YAxisMin}  {YAxisMax,-4}    {YAxsRMin,-4}  {YAxsRMax} \t   \n");    //await TickRepaintDelay();
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }
  }
  void DrawFore24hrEC(string site, siteData? sitedata)
  {
    try
    {
      switch (site)
      {
        default: break;
        case Cnst._markham: PlotViewModelHelpers.RefillForeEnvtCa(ECaMrkhTemp, sitedata); break;
        case Cnst._toronto: PlotViewModelHelpers.RefillForeEnvtCa(ECaToroTemp, sitedata); break;
        case Cnst._torIsld: PlotViewModelHelpers.RefillForeEnvtCa(ECaTIslTemp, sitedata); break;
        case Cnst._mississ: PlotViewModelHelpers.RefillForeEnvtCa(ECaMissTemp, sitedata); break;
        case Cnst._richmhl:
          PlotViewModelHelpers.RefillForeEnvtCa(ECaVghnTemp, sitedata);
          DrawTemperatureExtremesSetBoundsForAxisY();
          EnvtCaIconM = $"https://weather.gc.ca/weathericons/{sitedata?.currentConditions?.iconCode?.Value ?? "5":0#}.gif"; // img1.Source = new BitmapImage(new Uri($"https://weather.gc.ca/weathericons/{(sitedata?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"));
          break;
      }

      Model.InvalidatePlot(true); SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  Envt CA      Fore   \t{site.Substring(4, 3)}\t {YAxisMin}  {YAxisMax,-4}    {YAxsRMin,-4}  {YAxsRMax} \t   \n");
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }
  }
  void DrawWeatherIcons(RootobjectFrc5Day3Hr forecast5day3hr)
  {
    var h3 = 0; for (; h3 < OpenWea.UnixToDt(forecast5day3hr.list[0].dt).Hour / 3; h3++)
    {
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/01n.png", UriKind.Absolute)); //nogo: OpnWeaIco3[h3] = new BitmapImage(new Uri($"/Views/NoDataIndicator.bmp", UriKind.Absolute));
      OpnWeaTip3[h3] = $"{h3}";
    }

    forecast5day3hr.list.ToList().ForEach(r =>
    {
      //tmi: _lgr.Log(LogLevel.Trace, $"});d53:  {r.dt_txt}    {UnixToDt(r.dt)}    {UnixToDt(r.dt):MMM dd  HH:mm}     {r.weather[0].description,-26}       {r.main.temp_min,6:N1} ÷ {r.main.temp_max,4:N1}°    pop{r.pop * 100,3:N0}%      {r}");
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{r.weather[0].icon}@2x.png"));
      OpnWeaTip3[h3] = $"{OpenWea.UnixToDt(r.dt):MMM d  H:mm} \n\n    {r.weather[0].description}   \n    {r.main.temp:N1}°    pop {r.pop * 100:N0}%";
      h3++;
    });
  }
  void DrawMainMeteoMeasurementLines(RootobjectOneCallApi? openWeather, RootobjectFrc5Day3Hr forecast5day3hr)
  {
    forecast5day3hr.list.ToList().ForEach(x =>
      {
        //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(day0.dt)), day0.snow?._3h ?? 0, day0.snow?._3h ?? 0, _d3c));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));
        OwaLoclFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like));
        OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust * _ms2kh));
        OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));

        // OpenMeteo is good enough
        //var rad = Math.PI * x.wind.deg * 2 / 360;
        //var dx = 0.1 * Math.Cos(rad)*3;
        //var dy = 1.0 * Math.Sin(rad)*3;
        //OMtLoclWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _ms2kh));
        //OMtLoclWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + dx, dy + (x.wind.speed * _ms2kh)));
        //OMtLoclWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _ms2kh));
      });

    if (openWeather?.daily is not null) // extended forecast
      openWeather.daily.Where(d => d.dt > forecast5day3hr.list.Max(d => d.dt)).ToList().ForEach(x =>
      {
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.temp.morn));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.temp.day));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.temp.eve));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.temp.night));

        OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _ms2kh));
        OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));
      });
  }
  void DrawSunSinusoid(double sunrizeWithDate)
  {
    var amplitudeInDegC = 16;
    var sunsetLineCrossing = -.202; // -.202 is perfect for Dec 7 (was -.19)
    var isDaylightSaving = TimeZoneInfo.Local.IsDaylightSavingTime(_startedAt);
    var noon = sunsetLineCrossing - (isDaylightSaving ? 0 : (1.0 / 24.0)); // :summer; for winter add 1hr: -.19 + 1.0/24.    Solstice sunrize at 7:27.5 == new DateTime(2023,3,15,7, 27, 30).ToOADate(); = 0.3107638888888889 ..why not ~.19 ?

    var dAmplitude = amplitudeInDegC * Math.Sin((sunrizeWithDate - noon) * Math.PI * 2);

    SunSinusoid.Clear();
    FunctionSeries__(Math.Sin, sunrizeWithDate - 1.3, sunrizeWithDate + 17.4, .0125);
    void FunctionSeries__(Func<double, double> sin, double x0, double x1, double dx)
    {
      for (var t = x0; t <= x1 + (dx * 0.5); t += dx)
        SunSinusoid.Add(new DataPoint(t, dAmplitude - (amplitudeInDegC * sin((t - noon) * Math.PI * 2))));
    }
  }

  void ModelClearAdd(string note)
  {
    _lgr.Log(LogLevel.Trace, $"::: {note}");

    try
    {
      Model.Legends.Clear();
      Model.Legends.Add(new Legend { LegendTextColor = OxyColors.LightYellow, LegendPosition = LegendPosition.LeftMiddle, LegendMargin = 10, LegendBackground = OxyColor.FromArgb(0x30, 0, 10, 0) });

      ReCreateAxises(note); // throws without

      OxyColor
        ow5 = OxyColor.FromArgb(0x80, 0xe0, 0x00, 0xe0),
        owa = OxyColor.FromRgb(0xe0, 0x00, 0xe0),
        wng = OxyColor.FromRgb(0x10, 0x60, 0x10),
        wnd = OxyColor.FromRgb(0x00, 0x99, 0x00),
        vgn = OxyColor.FromRgb(0x00, 0x80, 0xff),
        mis = OxyColor.FromRgb(0x00, 0x80, 0x00),
        tot = OxyColor.FromRgb(0xb0, 0x80, 0x00),
        tit = OxyColor.FromRgb(0xd0, 0x00, 0x00),
        owx = OxyColor.FromRgb(0xff, 0x80, 0x00),
        owp = OxyColor.FromRgb(0x00, 0x00, 0xff),
        omp = OxyColor.FromRgb(0x00, 0x00, 0xff),
        prs = OxyColor.FromRgb(0xaa, 0xff, 0x00),
        omt = OxyColor.FromRgb(0xcc, 0xcc, 0x00);

      Model.Series.Clear();
      Model.Series.Add(new AreaSeries { ItemsSource = SunSinusoid, Color = OxyColor.FromArgb(0x80, 0xff, 0xff, 0x00), StrokeThickness = 0.5, Title = "SunRS Sin", YAxisKey = "yAxisL", Fill = OxyColor.FromArgb(0x08, 0xff, 0xff, 0x00) });
      //Model.Series.Add(new AreaSeries { ItemsSource = Sunrise_Set, Color = OxyColor.FromRgb(0x11, 0x11, 0x11), StrokeThickness = 0.0, Title = "SunRS Sqr", YAxisKey = "yAxisR" });

      Model.Series.Add(new AreaSeries { ItemsSource = OwaLoclPopr, Color = owp, StrokeThickness = 0.5, Title = "owa PoPr", InterpolationAlgorithm = IA, YAxisKey = "yAxisR", Fill = OxyColor.FromArgb(0x80, 0x00, 0x00, 0x90) });
      Model.Series.Add(new LineSeries { ItemsSource = OwaTempExtr, Color = owx, StrokeThickness = 1.0, Title = "owa Extr", LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclTemp, Color = owa, StrokeThickness = 1.5, Title = "owa Temp", InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclFeel, Color = owa, StrokeThickness = 0.5, Title = "owa Feel", InterpolationAlgorithm = IA });
      //l.Series.Add(new LineSeries { ItemsSource = OwaLoclGust, Color = wnd, StrokeThickness = 0.5, Title = "owa Gust", InterpolationAlgorithm = IA, YAxisKey = "yAxisR" });

      Model.Series.Add(new AreaSeries { ItemsSource = OMtLoclGust, Color = wng, StrokeThickness = 0.5, Title = "omt Gust", InterpolationAlgorithm = IA, YAxisKey = "yAxisR", Fill = OxyColor.FromArgb(0x20, 0x00, 0x60, 0x00) });
      Model.Series.Add(new AreaSeries { ItemsSource = OMtLoclPopr, Color = omp, StrokeThickness = 1.0, Title = "omt PoPr", InterpolationAlgorithm = IA, YAxisKey = "yAxisR", Fill = OxyColor.FromArgb(0x80, 0x00, 0x00, 0xb0) });
      Model.Series.Add(new LineSeries { ItemsSource = OMtLoclWind, Color = wnd, StrokeThickness = 1.5, Title = "omt Wind", /*                        */ YAxisKey = "yAxisR" });
      Model.Series.Add(new LineSeries { ItemsSource = OMtLoclTemp, Color = omt, StrokeThickness = 1.5, Title = "omt Temp", InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OMtLoclPrsr, Color = prs, StrokeThickness = 1.0, Title = "omt Prsr", InterpolationAlgorithm = IA, LineStyle = LineStyle.Dot });
      //Model.Series.Add(new LineSeries { ItemsSource = OMtTempExtr, Color = OxyColor.FromRgb(0x80, 0x00, 0xf0), StrokeThickness = 1.0, Title = "omt Extr", LineStyle = LineStyle.LongDashDotDot });
      //Model.Series.Add(new LineSeries { ItemsSource = OMtLoclFeel, Color = omt, StrokeThickness = 0.5, Title = "omt Feel", InterpolationAlgorithm = IA });

      //Model.Series.Add(new LineSeries { ItemsSource = ECaToroTemp, Color = tot, StrokeThickness = 5.0, Title = "ec TO T", LineStyle = LineStyle.LongDash });
      //Model.Series.Add(new LineSeries { ItemsSource = ECaTIslTemp, Color = tit, StrokeThickness = 1.0, Title = "ec TI T", LineStyle = LineStyle.Dot });
      //same as VA: Model.Series.Add(new LineSeries { ItemsSource = ECaMrkhTemp, Color = vgn, StrokeThickness = 2.0, Title = "ec MA T", LineStyle = LineStyle.Dash, InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = ECaMissTemp, Color = mis, StrokeThickness = 3.0, Title = "ec Mi T", LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = ECaVghnTemp, Color = vgn, StrokeThickness = 3.0, Title = "ec VA T", LineStyle = LineStyle.Dash });

      if (SctrPtTPFVgn.Count >= 0) Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFVgn, Title = "ECa vgn", MarkerStroke = vgn, MarkerFill = OxyColors.Transparent, MarkerType = MarkerType.Circle });
      //if (SctrPtTPFMis.Count >= 0) Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFMis, Title = "ECa mis", MarkerStroke = mis, MarkerFill = OxyColors.Transparent, MarkerType = MarkerType.Circle });
      if (SctrPtTPFOMt.Count >= 0) Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFOMt, Title = "OpMeteo", MarkerStroke = omt, MarkerFill = OxyColors.Transparent, MarkerType = MarkerType.Circle });
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }

    Model.InvalidatePlot(true); //SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  Model re-freshed\t▓  {note,-26}  \t\t   \n";
  }
  void ReCreateAxises(string note)
  {
    var _eee = OxyColor.FromRgb(0xe0, 0xe0, 0xe0);
    var _mjg = OxyColor.FromRgb(0x40, 0x40, 0x40);
    var _ccc = OxyColor.FromRgb(0xc0, 0xc0, 0xc0);
    var _mng = OxyColor.FromRgb(0x20, 0x20, 0x20);
    var dddD = "                  ddd d";
    Model.Axes.Clear();
    Model.Axes.Add(new DateTimeAxis { Minimum = TimeMin, Maximum = TimeMax, MajorStep = 1, MinorStep = .250, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, StringFormat = dddD, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid, MajorGridlineColor = _mjg, MinorGridlineColor = _mng, MinorTickSize = 4, TicklineColor = _ccc, Position = AxisPosition.Top });
    Model.Axes.Add(new DateTimeAxis { Minimum = TimeMin, Maximum = TimeMax, MajorStep = 1, MinorStep = .250, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, StringFormat = dddD, Position = AxisPosition.Bottom });
    Model.Axes.Add(new LinearAxis { Minimum = YAxisMin, Maximum = YAxisMax, MajorStep = 010, MinorStep = 01, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = _mjg, Key = "yAxisL", Title = "Temp [°C]", MinorTickSize = 4, TicklineColor = _ccc });
    Model.Axes.Add(new LinearAxis { Minimum = YAxsRMin, Maximum = YAxsRMax, MajorStep = 100, MinorStep = 10, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, Position = AxisPosition.Right, MajorGridlineStyle = LineStyle.None, MajorGridlineColor = _mng, Key = "yAxisR", Title = "Wind k/h  PoP %" });

    Model.InvalidatePlot(true);
    SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  Axiss re-adjustd\t■  {note,-26}  \n");
  }
  void SmartAdd(string note)
  {
    SubHeader += note;
    var max = 1800;
    var len = SubHeader.Length;
    if (len > max)
      SubHeader = $"   {_startedAt:ddd HH:mm}\t\n{SubHeader.Substring(len - max, max)}"; // cut off the excess from the beginning.
  }
  internal void ClearPlot()
  {
    Model.Legends.Clear();
    Model.Axes.Clear();
    Model.Series.Clear();
  }

  [RelayCommand] void Invalidate(object updateData) { bpr.Tick(); Model.InvalidatePlot(updateData?.ToString() == "true"); }
  [RelayCommand] void CreateMdl(object note) { bpr.Tick(); ModelClearAdd(note?.ToString() ?? "000"); }
  [RelayCommand]
  void ClearData()
  {
    bpr.Click();

    Model.Title = CurrentConditions = "";
    WindDirn = 0;
    WindVeloKmHr = 0;
    OpnWeaIcom = "http://openweathermap.org/img/wn/01n@2x.png";

    SctrPtTPFVgn.Clear();
    //SctrPtTPFMis.Clear();
    SctrPtTPFOMt.Clear();
    OwaLoclTemp.Clear();
    OwaLoclFeel.Clear();
    OwaLoclGust.Clear();
    SunSinusoid.Clear();
    OMtLoclWind.Clear();
    Sunrise_Set.Clear();
    OwaTempExtr.Clear();
    OwaLoclPopr.Clear();
    ECaToroTemp.Clear();
    ECaVghnTemp.Clear();
    ECaMrkhTemp.Clear();
    ECaMissTemp.Clear();
    ECaTIslTemp.Clear();

    Model.InvalidatePlot(true);
    SubHeader = "Data cleared \t\t\t\t\t   \n";
  }
  [RelayCommand]
  async Task GetDays(object? daysAhead)
  {
    if (int.TryParse(daysAhead?.ToString(), out var days))
    {
      TimeMax = DateTimeAxis.ToDouble(DateTime.Today.AddDays(days));

      IconWidth0 = 0 < days ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
      IconWidth1 = 1 < days ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
      IconWidth2 = 2 < days ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
      IconWidth3 = 3 < days ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
      IconWidth4 = 4 < days ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
      IconWidth5 = 5 < days ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
      IconWidth6 = 6 < days ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
      IconWidth7 = 7 < days ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
    }

    await Task.Delay(333); // ..Command.IsRunning demo.
  }

  #region icons
  [ObservableProperty] GridLength iconWidth0;
  [ObservableProperty] GridLength iconWidth1;
  [ObservableProperty] GridLength iconWidth2;
  [ObservableProperty] GridLength iconWidth3;
  [ObservableProperty] GridLength iconWidth4;
  [ObservableProperty] GridLength iconWidth5;
  [ObservableProperty] GridLength iconWidth6;
  [ObservableProperty] GridLength iconWidth7;
  [ObservableProperty] GridLength iconWidt00;
  [ObservableProperty] GridLength iconWidt01;
  [ObservableProperty] GridLength iconWidt02;
  [ObservableProperty] GridLength iconWidt03;
  [ObservableProperty] GridLength iconWidt04;
  [ObservableProperty] GridLength iconWidt05;
  [ObservableProperty] GridLength iconWidt06;
  [ObservableProperty] GridLength iconWidt07;
  [ObservableProperty] GridLength iconWidt08;
  [ObservableProperty] GridLength iconWidt09;
  [ObservableProperty] GridLength iconWidt10;
  [ObservableProperty] GridLength iconWidt11;
  [ObservableProperty] GridLength iconWidt12;
  [ObservableProperty] GridLength iconWidt13;
  [ObservableProperty] GridLength iconWidt14;
  [ObservableProperty] GridLength iconWidt15;
  [ObservableProperty] GridLength iconWidt16;
  [ObservableProperty] GridLength iconWidt17;
  [ObservableProperty] GridLength iconWidt18;
  [ObservableProperty] GridLength iconWidt19;
  [ObservableProperty] GridLength iconWidt20;
  [ObservableProperty] GridLength iconWidt21;
  [ObservableProperty] GridLength iconWidt22;
  [ObservableProperty] GridLength iconWidt23;
  [ObservableProperty] GridLength iconWidt24;
  [ObservableProperty] GridLength iconWidt25;
  [ObservableProperty] GridLength iconWidt26;
  [ObservableProperty] GridLength iconWidt27;
  [ObservableProperty] GridLength iconWidt28;
  [ObservableProperty] GridLength iconWidt29;
  [ObservableProperty] GridLength iconWidt30;
  [ObservableProperty] GridLength iconWidt31;
  [ObservableProperty] GridLength iconWidt32;
  [ObservableProperty] GridLength iconWidt33;
  [ObservableProperty] GridLength iconWidt34;
  [ObservableProperty] GridLength iconWidt35;
  [ObservableProperty] GridLength iconWidt36;
  [ObservableProperty] GridLength iconWidt37;
  [ObservableProperty] GridLength iconWidt38;
  [ObservableProperty] GridLength iconWidt39;
  [ObservableProperty] string opnWeaIcom = "http://openweathermap.org/img/wn/01d@2x.png";
  [ObservableProperty] string envtCaIconM = "https://weather.gc.ca/weathericons/05.gif";
  [ObservableProperty] string envtCaIconV = "https://weather.gc.ca/weathericons/05.gif";
  [ObservableProperty] string subHeader = "";
  [ObservableProperty] double normTMin = +15;
  [ObservableProperty] double normTMax = +25;
  [ObservableProperty] double yAxisMin = -18; partial void OnYAxisMinChanged(double value) => ReCreateAxises("Y min");
  [ObservableProperty] double yAxisMax = +12; partial void OnYAxisMaxChanged(double value) => ReCreateAxises("Y max");
  [ObservableProperty] double yAxsRMax = +180; partial void OnYAxsRMaxChanged(double value) => ReCreateAxises("Y max R");
  [ObservableProperty] double yAxsRMin = -120; partial void OnYAxsRMinChanged(double value) => ReCreateAxises("Y min R");
  [ObservableProperty] double windGustKmHr;
  [ObservableProperty] IInterpolationAlgorithm iA = InterpolationAlgorithms.CatmullRomSpline; // the least vertical jumping beyond y value.

  RootobjectOneCallApi? _openWeather;
  WeatherForecast? _openMeteo;
  siteData? _foreVgn, _foreMis;
  List<MeteoDataMy>? _pastPea, _pastKng;
  bool _isDbBusy;
  #endregion
}