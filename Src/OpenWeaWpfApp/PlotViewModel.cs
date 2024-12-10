#define ObsCol // Go figure: ObsCol works, while array NOT! Just an interesting factoid.
using OpenMeteoClient.Application.Interfaces;
using OpenMeteoClient.Domain.Models;

namespace OpenWeaWpfApp;
public partial class PlotViewModel : ObservableValidator
{
  #region fields
  readonly DateTime _startedAt = DateTime.Now;
  readonly Bpr bpr = new();
  readonly int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600;
  readonly IConfigurationRoot _cfg;

  //readonly WeatherxContext _dbx;
  readonly OpenWea _opnwea;
  private readonly IWeatherForecastService _openMeteoSvc;
  readonly DbxHelper _dbh;
  readonly ILogger _lgr;
  readonly SpeechSynth _synth;
  readonly bool _store;
  const int _maxIcons = 50;
  double _extrMax = +35, _extrMin = +05;

  //ImageSource _i; public ImageSource WeaIcom { get => _i; set => SetProperty(ref _i, value); }
  //Uri _k = new("http://openweathermap.org/img/wn/04n@2x.png"); public Uri WIcon { get => _k; set => SetProperty(ref _k, value); }
  const float _wk = 10f;
  const float _ms2kh = 3.6f * _wk;
  readonly ObservableCollection<ScatterPoint> SctrPtTPFVgn = [];
  readonly ObservableCollection<ScatterPoint> SctrPtTPFPhc = [];
  readonly ObservableCollection<ScatterPoint> SctrPtTPFMis = [];

  readonly ObservableCollection<DataPoint> OwaLoclTemp = [];
  readonly ObservableCollection<DataPoint> OwaLoclFeel = [];
  readonly ObservableCollection<DataPoint> OwaLoclPrsr = [];
  readonly ObservableCollection<DataPoint> OwaLoclGust = []; // not shown :tmi
  readonly ObservableCollection<DataPoint> OwaTempExtr = [];
  readonly ObservableCollection<DataPoint> OwaLoclPopr = [];

  readonly ObservableCollection<DataPoint> OMeLoclTemp = [];
  readonly ObservableCollection<DataPoint> OMeLoclFeel = []; //
  readonly ObservableCollection<DataPoint> OMeLoclPrsr = []; //
  readonly ObservableCollection<DataPoint> OMeLoclGust = [];
  readonly ObservableCollection<DataPoint> OMeTempExtr = []; //
  readonly ObservableCollection<DataPoint> OMeLoclPopr = [];

  readonly ObservableCollection<DataPoint> Sunrise_Set = [];
  readonly ObservableCollection<DataPoint> SunSinusoid = [];
  readonly ObservableCollection<DataPoint> OMeLoclWind = [];
  readonly ObservableCollection<DataPoint> ECaPearWind = [];
  readonly ObservableCollection<DataPoint> ECaToroTemp = [];
  readonly ObservableCollection<DataPoint> ECaVghnTemp = [];
  readonly ObservableCollection<DataPoint> ECaMrkhTemp = [];
  readonly ObservableCollection<DataPoint> ECaMissTemp = [];
  readonly ObservableCollection<DataPoint> ECaTIslTemp = [];

  [Browsable(false)][ObservableProperty] PlotModel model = new() { TextColor = OxyColors.GreenYellow }; //void OnModelChanged() => PropertiesChanged();  void PropertiesChanged() => Model = ModelClearAdd("Prop Chgd");
  [ObservableProperty] double timeMin = DateTime.Today.ToOADate() - 1; partial void OnTimeMinChanged(double value) => ReCreateAxises("T min");
  [ObservableProperty] double timeMax = DateTime.Today.ToOADate() + 2; partial void OnTimeMaxChanged(double value) => ReCreateAxises("T max");
  [ObservableProperty] string plotTitle = "";
  [ObservableProperty] string currentConditions = "";
  [ObservableProperty] string curTempReal = "";
  [ObservableProperty] string curTempFeel = "";
  [ObservableProperty] string curWindKmHr = "";
  [ObservableProperty] int windDirn;
  [ObservableProperty] float windVeloKmHr;
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

  public PlotViewModel(OpenWea openWea, IWeatherForecastService openMeteo, DbxHelper dbh, ILogger lgr, SpeechSynth synth, IConfigurationRoot cfg)
  {
    _cfg = cfg; // _cfg = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    _dbh = dbh;
    _lgr = lgr;
    _synth = synth;
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
  [RelayCommand]
  public void PopulateAll(object? obj)
  {
    if (obj is null) bpr.Start();

    try
    {
      //SmartAdd($"*** {_dbx.Database.GetConnectionString()} ***\n"); // 480ms
      PrevForecastFromDb(obj);
      PopulateScatModel(obj);

      if (obj is null) bpr.Finish();
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }
  }
  [RelayCommand]
  void PrevForecastFromDb(object? obj)
  {
    if (obj is null) bpr.Click();
    if (!_store) return;
    try
    {
      _ = Task.Run(GetPastForecastFromDB).ContinueWith(_ =>
      {
        _.Result.a.ForEach(r => SctrPtTPFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
        _.Result.b.ForEach(r => SctrPtTPFVgn.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
        _.Result.c.ForEach(r => SctrPtTPFMis.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
        Model.InvalidatePlot(true); SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  Populated: From DB \t\t\t\t\t   \n");
        bpr.Tick();
      }, TaskScheduler.FromCurrentSynchronizationContext());
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }
  }
  [RelayCommand]
  async Task PopulateScatModel(object? obj)
  {
    if (obj is null) bpr.Click();

    _ = Task.Run(async () => await PlotViewModelHelpers.GetPast24hrFromEC(Cnst._Past24YYZ)).ContinueWith(_ => { DrawPast24hrEC(Cnst.pearson, _.Result); _pastPea = _.Result; }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetPast24hrFromEC(Cnst._Past24OKN)).ContinueWith(_ => { DrawPast24hrEC(Cnst.batnvil, _.Result); _pastBvl = _.Result; }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._mississ)).ContinueWith(_ => { DrawFore24hrEC(Cnst._mississ, _.Result); _foreMis = _.Result; Model.Title = $"     Miss {(_foreMis?.currentConditions)?.dateTime[1].hour.Value}:{(_foreMis?.currentConditions)?.dateTime[1].minute} {float.Parse(_foreMis?.currentConditions?.temperature?.Value ?? "-999F"),5:+##.#;-##.#;0}° {float.Parse(_foreMis?.currentConditions?.windChill?.Value ?? _foreMis?.currentConditions?.temperature?.Value ?? "-999"),4:+##;-##;0}° {_foreMis?.currentConditions?.wind?.speed?.Value,4} {(_foreMis?.currentConditions?.wind)?.speed.units}      {Model?.Title}  "; }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._richmhl)).ContinueWith(_ => { DrawFore24hrEC(Cnst._richmhl, _.Result); _foreVgn = _.Result; Model.Title = $"     Vaug {(_foreVgn?.currentConditions)?.dateTime[1].hour.Value}:{(_foreVgn?.currentConditions)?.dateTime[1].minute} {float.Parse(_foreVgn?.currentConditions?.temperature?.Value ?? "-999F"),5:+##.#;-##.#;0}° {float.Parse(_foreVgn?.currentConditions?.windChill?.Value ?? _foreVgn?.currentConditions?.temperature?.Value ?? "-999"),4:+##;-##;0}° {_foreVgn?.currentConditions?.wind?.speed?.Value,4} {(_foreVgn?.currentConditions?.wind)?.speed.units}      {Model?.Title}  "; }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._toronto)).ContinueWith(_ => { DrawFore24hrEC(Cnst._toronto, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._torIsld)).ContinueWith(_ => { DrawFore24hrEC(Cnst._torIsld, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._markham)).ContinueWith(_ => { DrawFore24hrEC(Cnst._markham, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());

    //_ = Task.Run<object>(async () => await _openMeteoSvc.GetForecastAsync(43.83, -79.5) ?? throw new ArgumentNullException(nameof(obj))).ContinueWith(async _ =>
    {
      //_openMeteo = _.Result as WeatherForecast;

      _openMeteo = await _openMeteoSvc.GetForecastAsync(43.83, -79.5);

      ArgumentNullException.ThrowIfNull(_openMeteo);

      Model.Title = $"     OMe {(_openMeteo.Current.Time):HH:mm} {_openMeteo.Current.Temperature2m,5:+##.#;-##.#;0}° {_openMeteo.Current.ApparentTemperature,4:+##;-##;0}° {_openMeteo.Current.WindSpeed10m * _ms2kh / _wk,4:N0} k/h       {Model.Title}";
      WindDirn = _openMeteo.Current.WindDirection10m;
      WindVeloKmHr = (float)(_openMeteo.Current.WindSpeed10m); //  * _ms2kh / _wk;
      WindGustKmHr = _openMeteo.Current.WindGusts10m; //  * _ms2kh / _wk;
      CurTempReal = $"{_openMeteo.Current.Temperature2m:+#.#;-#.#;0}°";
      CurTempFeel = $"{_openMeteo.Current.ApparentTemperature:+#;-#;0}°";
      CurWindKmHr = $"{WindVeloKmHr:N1}";

      for (int i = 0; i < _openMeteo.Hourly.Time.Count; i++)
      {
        var t = _openMeteo.Hourly.Time[i].ToOADate();

        OMeLoclTemp.Add(new DataPoint(t, _openMeteo.Hourly.Temperature2m[i]));
        //OMeLoclFeel.Add(new DataPoint(t, _openMeteo.Hourly.app));
        //OMeLoclPrsr.Add(new DataPoint(t, _openMeteo.Hourly.pressure - 1030));
        OMeLoclGust.Add(new DataPoint(t, _openMeteo.Hourly.WindGusts10m[i] * 10));
        OMeLoclPopr.Add(new DataPoint(t, _openMeteo.Hourly.PrecipitationProbability[i]));

        var rad = Math.PI * _openMeteo.Hourly.WindDirection10m[i] * 2 / 360;
        var dx = 0.10 * Math.Cos(rad);
        var dy = 10.0 * Math.Sin(rad);
        var tx = .002 * Math.Cos(rad + 90);
        var ty = 0.20 * Math.Sin(rad + 90);
        var sx = .002 * Math.Cos(rad - 90);
        var sy = 0.20 * Math.Sin(rad - 90);
        OMeLoclWind.Add(new DataPoint(t + tx, ty + (_openMeteo.Hourly.WindSpeed10m[i] * _wk)));
        OMeLoclWind.Add(new DataPoint(t + dx, dy + (_openMeteo.Hourly.WindSpeed10m[i] * _wk)));
        OMeLoclWind.Add(new DataPoint(t + sx, sy + (_openMeteo.Hourly.WindSpeed10m[i] * _wk)));
      }

      for (int i = 0; i < _openMeteo.Daily.Sunrise.Count; i++)
      {
        var sunR = _openMeteo.Daily.Sunrise[i].ToOADate();
        var sunS = _openMeteo.Daily.Sunset[i].ToOADate();
        Sunrise_Set.Add(new DataPoint(sunR, -000));
        Sunrise_Set.Add(new DataPoint(sunR, +800));
        Sunrise_Set.Add(new DataPoint(sunS, +800));
        Sunrise_Set.Add(new DataPoint(sunS, -000));
      }

      DrawSunSinusoid(_openMeteo.Daily.Sunrise.First().ToOADate()); // DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 7.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 6.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 5.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 4.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 3.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);       DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 2.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 1.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 0.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 1.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 2.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 3.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 4.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 5.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 6.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 7.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        */      }

      await GetDays(7); // 8 is too much: last day is empty of data (2024-12)

      Model.InvalidatePlot(true);
      SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  OMe  \n");

      //await DelayedStoreToDbIf(); // ERR: A second operation was started on this context instance before a previous operation completed. 

    }//, TaskScheduler.FromCurrentSynchronizationContext());

    //_ = Task.Run<object>(async () => await _opnwea.GetIt__(_cfg["AppSecrets:MagicWeather"]!, OpenWea.OpenWeatherCd.OneCallApi) ?? throw new ArgumentNullException(nameof(obj))).ContinueWith(async _ =>
    {
      _openWeather = await _opnwea.GetIt__(_cfg["AppSecrets:MagicWeather"]!, OpenWea.OpenWeatherCd.OneCallApi) as RootobjectOneCallApi; //_.Result as RootobjectOneCallApi; ArgumentNullException.ThrowIfNull(_openWeather); // PHC107

      if (_openWeather.current is null) // always null since became a paid service in 2024!!!!!
      {
        //Model.Title = $"_openWeather.current is null ■ {Model.Title}";
        //WindDirn = 0;
        //WindVeloKmHr = 0;
        //WindGustKmHr = 0;
        //CurTempReal = $"~~~";
        //CurTempFeel = $"~~~";
        //CurWindKmHr = $"~~~";
        //OpnWeaIcom = $"~~~";
      }
      else //todo: replace assignments with alternative sources:
      {
        Model.Title = $"OWA {OpenWea.UnixToDt(_openWeather.current.dt):HH:mm} {_openWeather.current.temp,5:+##.#;-##.#;0}° {_openWeather.current.feels_like,4:+##;-##;0}° {_openWeather.current.wind_speed * _ms2kh / _wk,5:N1} k/h   \t {Model.Title}";
        WindDirn = _openWeather.current.wind_deg;
        WindVeloKmHr = _openWeather.current.wind_speed * _ms2kh / _wk;
        WindGustKmHr = _openWeather.current.wind_gust * _ms2kh / _wk;
        CurTempReal = $"{_openWeather.current.temp:+#.#;-#.#;0}°";
        CurTempFeel = $"{_openWeather.current.feels_like:+#;-#;0}°";
        CurWindKmHr = $"{WindVeloKmHr:N1}";
        OpnWeaIcom = $"http://openweathermap.org/img/wn/{_openWeather.current.weather.First().icon}@2x.png";

        for (var i = 0; i < _openWeather.daily.Length; i++) OpnWeaIcoA.Add($"http://openweathermap.org/img/wn/{_openWeather.daily[i].weather[0].icon}@2x.png");

        _openWeather.hourly.ToList().ForEach(x =>
        {
          OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp));
          OwaLoclFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.feels_like));
          OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pressure - 1030));
          OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _ms2kh));
          OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));
        });

        var day0 = _openWeather.daily.First();
        Sunrise_Set.Add(new DataPoint(OpenWea.UnixToDt(day0.sunrise).AddDays(-1).ToOADate(), -000));
        Sunrise_Set.Add(new DataPoint(OpenWea.UnixToDt(day0.sunrise).AddDays(-1).ToOADate(), +800));
        Sunrise_Set.Add(new DataPoint(OpenWea.UnixToDt(day0.sunset).AddDays(-1).ToOADate(), +800));
        Sunrise_Set.Add(new DataPoint(OpenWea.UnixToDt(day0.sunset).AddDays(-1).ToOADate(), -000));
        _openWeather.daily.ToList().ForEach(x =>
        {
          Sunrise_Set.Add(new DataPoint(OpenWea.UnixToDt(x.sunrise).ToOADate(), -000));
          Sunrise_Set.Add(new DataPoint(OpenWea.UnixToDt(x.sunrise).ToOADate(), +800));
          Sunrise_Set.Add(new DataPoint(OpenWea.UnixToDt(x.sunset).ToOADate(), +800));
          Sunrise_Set.Add(new DataPoint(OpenWea.UnixToDt(x.sunset).ToOADate(), -000));
        });

        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate());        /*        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 7.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 6.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 5.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 4.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 3.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);       DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 2.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() - 1.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 0.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 1.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 2.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 3.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 4.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 5.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 6.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        DrawSunSinusoid(OpenWea.UnixToDt(day0.sunrise).ToOADate() + 7.0 / 24);    await Task.Delay(2_00); Model.InvalidatePlot(true); await Task.Delay(2_00); Model.InvalidatePlot(true);        */
      }

      var forecast5day3hr = await _opnwea.GetIt__(_cfg["AppSecrets:MagicWeather"]!, OpenWea.OpenWeatherCd.Frc5Day3Hr) as RootobjectFrc5Day3Hr; // FREE
      if (forecast5day3hr != null)
      {
        DrawWeatherIcons(forecast5day3hr);
        DrawMainMeteoMeasurementLines(_openWeather, forecast5day3hr);
      }

      await GetDays(7);
      Model.InvalidatePlot(true);
      SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  OWA  \n");
    }//, TaskScheduler.FromCurrentSynchronizationContext());

    await DelayedStoreToDbIf();
  }

  async Task<(List<PointFore> a, List<PointFore> b, List<PointFore> c)> GetPastForecastFromDB()
  {
    while (_isDbBusy)
    {
      await bpr.WarnAsync(); // not quite a full solution
    }

    _isDbBusy = true;
    try
    {
      var now = DateTime.Now;
      var ytd = now.AddHours(-24);
      var dby = now.AddHours(-48);
      var dbx = _dbh.WeatherxContext;

      //if (Environment.UserDomainName != "RAZER1") try { _dbx.EnsureCreated22(); } catch (Exception ex) { _lgr.Log(LogLevel.Trace, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }
      //_lgr.Log(LogLevel.Trace, $"■97 {dbx.Database.GetConnectionString()}"); // 480ms

      var phc = await dbx.PointFore.Where(r => r.SiteId == Cnst._phc && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync();
      var vgn = await dbx.PointFore.Where(r => r.SiteId == Cnst._vgn && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync();
      var mis = await dbx.PointFore.Where(r => r.SiteId == Cnst._mis && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync();

      return (phc, vgn, mis);
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■88 {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
      throw;
    }
    finally { _isDbBusy = false; }
  }
  void GetImprtandDatFromPearson(siteData? sitedata)
  {
#if ResaveToHardcodedField // Jun 2024
    using var fileStream = new FileStream(@"C:\g\Radar\Src\OpenWeaWpfApp\weather.gc.ca\en_climate_almanac_ON_6158733.xml", FileMode.Open);
    var almanacAll = ((climatedata?)new XmlSerializer(typeof(climatedata)).Deserialize(fileStream));
    Clipboard.SetText(JsonStringSerializer.Save(almanacAll)); // paste to ClimatedataStore.Json
#else
    var almanacAll = JsonStringSerializer.Load<climatedata>(ClimatedataStore.Json);
#endif

    var almanac = almanacAll?.month[DateTime.Today.Month - 1].day[DateTime.Today.Day - 1]; // ~50 ms

    ArgumentNullException.ThrowIfNull(almanac, $"@1232 {nameof(sitedata)}");

    _extrMax = (double)almanac.temperature[0].Value;
    _extrMin = (double)almanac.temperature[1].Value;
    NormTMax = (double)almanac.temperature[2].Value;
    NormTMin = (double)almanac.temperature[3].Value;

    const int pad = 2;
    YAxisMin = (Math.Floor((_extrMax - 50) / 10) * 10) - pad;
    YAxisMax = _extrMax + 5;

    YAxsRMin = -10 * pad;
    YAxsRMax = (YAxisMax - YAxisMin - pad) * 10;

    var now = DateTime.Now;
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

    ArgumentNullException.ThrowIfNull(sitedata, $"@@@@@@@@@ {nameof(sitedata)}");
    EnvtCaIconM = $"https://weather.gc.ca/weathericons/{sitedata?.currentConditions?.iconCode?.Value ?? "5":0#}.gif"; // img1.Source = new BitmapImage(new Uri($"https://weather.gc.ca/weathericons/{(sitedata?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"));
  }
  async Task<bool> DelayedStoreToDbIf()
  {
    if (!_store || VersionHelper.IsDbg)
      return false;

    await Task.Delay(120_000);

    var timeSinceLastDbStor2 = await PlotViewModelHelpers.LastTimeStoredToDb(_lgr, _dbh.WeatherxContext);
    var timeSinceLastDbStore = DateTimeOffset.Now - timeSinceLastDbStor2;
    if (timeSinceLastDbStore.TotalHours < 9)
    {
      //tmi: _synth.SpeakFreeFAF($"Too soon to store to DB: only {timeSinceLastDbStore.TotalHours:N1} hours passed.", volumePercent: 10); // _synth.SpeakFAF($"Too soon to store to DB: less than 6 hours passed.", volumePercent: 10);
      return false;
    }

    try
    {
      await PlotViewModelHelpers.AddPast24hrToDB_EnvtCa(_dbh.WeatherxContext, Cnst.pearson, _pastPea);
      await PlotViewModelHelpers.AddPast24hrToDB_EnvtCa(_dbh.WeatherxContext, Cnst.batnvil, _pastBvl);
      await PlotViewModelHelpers.AddForecastToDB_EnvtCa(_lgr, _dbh.WeatherxContext, Cnst._mis, _foreMis);
      await PlotViewModelHelpers.AddForecastToDB_EnvtCa(_lgr, _dbh.WeatherxContext, Cnst._vgn, _foreVgn);
      //todo:
      //await PlotViewModelHelpers.AddForecastToDB_OpnWea(_dbh.WeatherxContext, Cnst._phc, _openWeather);
      await PlotViewModelHelpers.AddForecastToDB_OpnMto(_dbh.WeatherxContext, Cnst._phc, _openMeteo);

      //tmi: _synth.SpeakFAF("All stored to DB.", volumePercent: 10);

      SmartAdd($"{(DateTime.Now - _startedAt).TotalSeconds,6:N1}\t  All stored to DB! \n");

      return true;
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr); //_lgr.Log(LogLevel.Error, $"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show($"{ex} ■ ■ ■", $"■ ■ ■ {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
      throw;
    }
  }
  void DrawPast24hrEC(string site, List<MeteoDataMy> lst)
  {
    try
    {
      switch (site)
      {
        //tmi: case Cnst.pearson: PlotViewModelHelpers.RefillPast24(ECaMissTemp, OMeLoclWind, lst, _wk); break;
        case Cnst.batnvil: PlotViewModelHelpers.RefillPast24(ECaVghnTemp, ECaPearWind, lst, _wk); break;
        default: break;
      }

      //todo: OwaLoclPrsr.Clear();    dta.OrderBy(r => r.TakenAt).ToList().ForEach(x => OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), (10 * x.Pressure) - 1030)));

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
        case Cnst._toronto: PlotViewModelHelpers.RefillForeEnvtCa(ECaToroTemp, sitedata); break;
        case Cnst._torIsld: PlotViewModelHelpers.RefillForeEnvtCa(ECaTIslTemp, sitedata); break;
        case Cnst._mississ: PlotViewModelHelpers.RefillForeEnvtCa(ECaMissTemp, sitedata); GetImprtandDatFromPearson(sitedata); break;
        case Cnst._richmhl: PlotViewModelHelpers.RefillForeEnvtCa(ECaVghnTemp, sitedata); break;
        case Cnst._markham: PlotViewModelHelpers.RefillForeEnvtCa(ECaMrkhTemp, sitedata); break;
        default: break;
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
  void DrawMainMeteoMeasurementLines(RootobjectOneCallApi openWeather, RootobjectFrc5Day3Hr forecast5day3hr)
  {
    forecast5day3hr.list.ToList().ForEach(x =>
      {
        //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(day0.dt)), day0.snow?._3h ?? 0, day0.snow?._3h ?? 0, _d3c));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));
        OwaLoclFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like));
        OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.pressure - 1030));
        OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust * _ms2kh));
        OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));

        // OpenMeteo is good enough
        //var rad = Math.PI * x.wind.deg * 2 / 360;
        //var dx = 0.1 * Math.Cos(rad)*3;
        //var dy = 1.0 * Math.Sin(rad)*3;
        //OMeLoclWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _ms2kh));
        //OMeLoclWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + dx, dy + (x.wind.speed * _ms2kh)));
        //OMeLoclWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _ms2kh));
      });

    if (openWeather.daily is not null) // extended forecast
      openWeather.daily.Where(d => d.dt > forecast5day3hr.list.Max(d => d.dt)).ToList().ForEach(x =>
      {
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.temp.morn));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.temp.day));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.temp.eve));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.temp.night));

        OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pressure - 1030));
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
      Model.Legends.Add(new Legend { LegendTextColor = OxyColors.LightGray, LegendPosition = LegendPosition.LeftMiddle, LegendMargin = 10, LegendBackground = OxyColor.FromRgb(0x10, 0x10, 0x10) });

      ReCreateAxises(note); // throws without

      OxyColor           
        _wnd = OxyColor.FromArgb(0x80, 0x77, 0x77, 0xaa),            
        _Vgn = OxyColor.FromRgb(0x00, 0x80, 0xff),
        _Mis = OxyColor.FromRgb(0x00, 0x80, 0x00);

      Model.Series.Clear();
      Model.Series.Add(new AreaSeries { ItemsSource = SunSinusoid, Color = OxyColor.FromArgb(0x80, 0xff, 0xff, 0x00), StrokeThickness = 0.5, Title = "SunRS Sin", YAxisKey = "yAxisL", Fill = OxyColor.FromArgb(0x08, 0xff, 0xff, 0x00) });
      //Model.Series.Add(new AreaSeries { ItemsSource = Sunrise_Set, Color = OxyColor.FromRgb(0x11, 0x11, 0x11), StrokeThickness = 0.0, Title = "SunRS Sqr", YAxisKey = "yAxisR" });

      Model.Series.Add(new LineSeries { ItemsSource = ECaToroTemp, Color = OxyColor.FromRgb(0xb0, 0x80, 0x00), StrokeThickness = 5.0, Title = "ec TO T", LineStyle = LineStyle.LongDash });
      Model.Series.Add(new LineSeries { ItemsSource = ECaMissTemp, Color = _Mis, StrokeThickness = 3.0, Title = "ec Mi T", LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = ECaTIslTemp, Color = OxyColor.FromRgb(0xd0, 0x00, 0x00), StrokeThickness = 1.0, Title = "ec TI T", LineStyle = LineStyle.Dot });
      Model.Series.Add(new LineSeries { ItemsSource = ECaVghnTemp, Color = _Vgn, StrokeThickness = 3.0, Title = "ec VA T", LineStyle = LineStyle.Dash });
      Model.Series.Add(new LineSeries { ItemsSource = ECaMrkhTemp, Color = _Vgn, StrokeThickness = 2.0, Title = "ec MA T", LineStyle = LineStyle.Dash, InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = ECaPearWind, Color = _wnd, StrokeThickness = 0.5, Title = "ec Wind Pea", YAxisKey = "yAxisR" });

      Model.Series.Add(new AreaSeries { ItemsSource = OwaLoclPopr, Color = OxyColor.FromArgb(0x80, 0x00, 0x20, 0x70), StrokeThickness = 0.0, Title = "owa PoPr", InterpolationAlgorithm = IA, YAxisKey = "yAxisR", Fill = OxyColor.FromArgb(0x80, 0x00, 0x20, 0x70) });
      Model.Series.Add(new LineSeries { ItemsSource = OwaTempExtr, Color = OxyColor.FromArgb(0x80, 0x40, 0x00, 0xa0), StrokeThickness = 1.0, Title = "owa Extr", LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclTemp, Color = OxyColor.FromArgb(0x80, 0xe0, 0x00, 0xe0), StrokeThickness = 1.5, Title = "owa Temp", InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclFeel, Color = OxyColor.FromArgb(0x80, 0xe0, 0x00, 0xe0), StrokeThickness = 0.5, Title = "owa Feel", InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclPrsr, Color = OxyColor.FromArgb(0x80, 0x60, 0x60, 0x00), StrokeThickness = 1.0, Title = "owa Prsr", InterpolationAlgorithm = IA, LineStyle = LineStyle.LongDashDotDot });
      //tmi: Model.Series.Add(new LineSeries { ItemsSource = OwaLoclGust, Color = _wnd, StrokeThickness = 0.5, Title = "owa Gust", InterpolationAlgorithm = IA, YAxisKey = "yAxisR" });

      Model.Series.Add(new AreaSeries { ItemsSource = OMeLoclPopr, Color = OxyColor.FromRgb(0x00, 0x00, 0xf0), StrokeThickness = 0.0, Title = "ome PoPr", InterpolationAlgorithm = IA, YAxisKey = "yAxisR", Fill = OxyColor.FromArgb(0x80, 0x00, 0x00, 0xf0) });
      //Model.Series.Add(new LineSeries { ItemsSource = OMeTempExtr, Color = OxyColor.FromRgb(0x80, 0x00, 0xf0), StrokeThickness = 1.0, Title = "ome Extr", LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = OMeLoclTemp, Color = OxyColor.FromRgb(0xcc, 0xcc, 0x00), StrokeThickness = 1.5, Title = "ome Temp", InterpolationAlgorithm = IA });
      //Model.Series.Add(new LineSeries { ItemsSource = OMeLoclFeel, Color = OxyColor.FromRgb(0xcc, 0xcc, 0x00), StrokeThickness = 0.5, Title = "ome Feel", InterpolationAlgorithm = IA });
      //Model.Series.Add(new LineSeries { ItemsSource = OMeLoclPrsr, Color = OxyColor.FromRgb(0xb0, 0xb0, 0x00), StrokeThickness = 1.0, Title = "ome Prsr", InterpolationAlgorithm = IA, LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = OMeLoclWind, Color = _wnd, StrokeThickness = 0.5, Title = "ome Wind", YAxisKey = "yAxisR"/*, Fill = OxyColor.FromArgb(0x20, 0x00, 0xff, 0x80)*/ });
      Model.Series.Add(new LineSeries { ItemsSource = OMeLoclGust, Color = _wnd, StrokeThickness = 0.5, Title = "ome Gust", InterpolationAlgorithm = IA, YAxisKey = "yAxisR" });

      Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFVgn, MarkerFill = OxyColors.Transparent, Title = "ECa _Vgn", MarkerType = MarkerType.Circle, MarkerStroke = _Vgn });
      Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFMis, MarkerFill = OxyColors.Transparent, Title = "ECa _Mis", MarkerType = MarkerType.Circle, MarkerStroke = _Mis });
      // crashes the model silently: Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFPhc, MarkerFill = OxyColors.Transparent, Title = "OWA OxyColor.FromArgb(0x80, 0xe0, 0x00, 0xe0)", MarkerType = MarkerType.Circle, MarkerStroke = OxyColor.FromArgb(0x80, 0xe0, 0x00, 0xe0), TrackerFormatString = "{}{0}&#xA;Time:   {2:HH:mm} &#xA;Temp:  {4:0.0}° " });
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
    var max = 800;
    var len = SubHeader.Length;
    if (len > max)
      SubHeader = $"   {_startedAt:ddd HH:mm}\t{SubHeader.Substring(len - max, max)}"; // cut off the excess from the beginning.
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
    SctrPtTPFPhc.Clear();
    SctrPtTPFMis.Clear();
    OwaLoclTemp.Clear();
    OwaLoclFeel.Clear();
    OwaLoclPrsr.Clear();
    OwaLoclGust.Clear();
    SunSinusoid.Clear();
    OMeLoclWind.Clear();
    ECaPearWind.Clear();
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
  List<MeteoDataMy>? _pastPea, _pastBvl;
  bool _isDbBusy;
  #endregion
}