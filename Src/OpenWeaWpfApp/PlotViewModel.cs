#define ObsCol // Go figure: ObsCol works, while array NOT! Just an interesting factoid.
using AmbienceLib;

namespace OpenWeaWpfApp;
public partial class PlotViewModel : ObservableValidator
{
  #region fields
  readonly DateTime _now = DateTime.Now;
  readonly Bpr bpr = new();
  readonly int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600, _vOffsetWas200 = 300, _yHi = 2, _yLo = 0; // 0 works for winter
  readonly IConfigurationRoot _cfg;
  //readonly WeatherxContext _dbx;
  readonly OpenWea _opnwea;
  readonly DbxHelper _dbh;
  private readonly ILogger _lgr;
  readonly bool _store;
  const int _maxIcons = 50;
  double _extrMax = +20, _extrMin = -20;
  readonly OxyColor _550f = OxyColor.FromArgb(0x50, 0x50, 0x00, 0xff),
   _111 = OxyColor.FromRgb(0x10, 0x10, 0x10),
   _mng = OxyColor.FromRgb(0x20, 0x20, 0x20),
   _330 = OxyColor.FromRgb(0x30, 0x30, 0x00),
   _mjg = OxyColor.FromRgb(0x40, 0x40, 0x40),
   _aaa = OxyColor.FromRgb(0xa0, 0xa0, 0xa0),
   _cc0 = OxyColor.FromRgb(0xc0, 0xc0, 0x00),
   _ccc = OxyColor.FromRgb(0xc0, 0xc0, 0xc0),
   _eee = OxyColor.FromRgb(0xe0, 0xe0, 0xe0),
   _b80 = OxyColor.FromRgb(0xb0, 0x80, 0x00),
   _d00 = OxyColor.FromRgb(0xd0, 0x00, 0x00),
   _Vgn = OxyColor.FromRgb(0x00, 0x80, 0xff),
   _Mis = OxyColor.FromRgb(0x00, 0x80, 0x00),
   _Phc = OxyColor.FromRgb(0xe0, 0x00, 0xe0),
   _Prs = OxyColor.FromRgb(0xb0, 0xb0, 0x00),
   _PoP = OxyColor.FromRgb(0x00, 0x40, 0xf0),
   _wnd = OxyColor.FromRgb(0x80, 0x80, 0x80);

  //ImageSource _i; public ImageSource WeaIcom { get => _i; set => SetProperty(ref _i, value); }
  //Uri _k = new("http://openweathermap.org/img/wn/04n@2x.png"); public Uri WIcon { get => _k; set => SetProperty(ref _k, value); }
  const float _wk = 10f;
  const float _ms2kh = 3.6f * _wk;
  readonly ObservableCollection<ScatterPoint> SctrPtTPFVgn = new();
  readonly ObservableCollection<ScatterPoint> SctrPtTPFPhc = new();
  readonly ObservableCollection<ScatterPoint> SctrPtTPFMis = new();
  readonly ObservableCollection<DataPoint> OwaLoclTemp = new();
  readonly ObservableCollection<DataPoint> OwaLoclFeel = new();
  readonly ObservableCollection<DataPoint> OwaLoclPrsr = new();
  readonly ObservableCollection<DataPoint> OwaLoclGust = new();
  readonly ObservableCollection<DataPoint> SunSinusoid = new();
  readonly ObservableCollection<DataPoint> ECaBtvlWind = new();
  readonly ObservableCollection<DataPoint> ECaPearWind = new();
  readonly ObservableCollection<DataPoint> OwaLoclSunT = new();
  readonly ObservableCollection<DataPoint> OwaLoclNowT = new();
  readonly ObservableCollection<DataPoint> OwaLoclPopr = new();
  readonly ObservableCollection<DataPoint> ECaToroTemp = new();
  readonly ObservableCollection<DataPoint> ECaVghnTemp = new();
  readonly ObservableCollection<DataPoint> ECaMrkhTemp = new();
  readonly ObservableCollection<DataPoint> ECaMissTemp = new();
  readonly ObservableCollection<DataPoint> ECaTIslTemp = new();
  [ObservableProperty] double timeMin = DateTime.Today.ToOADate() - 1; partial void OnTimeMinChanged(double value) => ReCreateAxises("T min");
  [ObservableProperty] double timeMax = DateTime.Today.ToOADate() + 2; partial void OnTimeMaxChanged(double value) => ReCreateAxises("T max");
  [ObservableProperty] string plotTitle = "";
  [ObservableProperty] string currentConditions = "";
  [ObservableProperty] string curTempReal = "";
  [ObservableProperty] string curTempFeel = "";
  [ObservableProperty] string curWindKmHr = "";
  [ObservableProperty] int windDirn;
  [ObservableProperty] float windVeloKmHr;
  [ObservableProperty] ObservableCollection<string> opnWeaIcoA = new();
#if ObsCol
  [ObservableProperty] ObservableCollection<ImageSource> opnWeaIco3 = new();
  [ObservableProperty] ObservableCollection<string> opnWeaTip3 = new();
#else
  ImageSource[] _3 = new ImageSource[_maxIcons]; public ImageSource[] OpnWeaIco3 { get => _3; set => SetProperty(ref _3, value); }
  string[] _4 = new string[_maxIcons]; public string[] OpnWeaTip3 { get => _4; set => SetProperty(ref _4, value); }
  //Messages _m3 = new(); public Messages OpnWeaIco3 { get => _m3; set => SetProperty(ref _m3, value); }
  //Messages _m4 = new(); public Messages OpnWeaTip3 { get => _m4; set => SetProperty(ref _m4, value); }
#endif
  #endregion

  public PlotViewModel(/*WeatherxContext weatherxContext,*/ OpenWea openWea, DbxHelper dbh, ILogger lgr)
  {
    _cfg = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    //_dbx = weatherxContext; // WriteLine($"*** {_dbx.Database.GetConnectionString()}"); // 480ms
    _dbh = dbh;
    this._lgr = lgr;
    _opnwea = openWea;
    _store = _cfg["StoreData"] == "Yes";

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
    _lgr.LogInformation("▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀ EOCtor");
  }

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
    catch (Exception ex) { WriteLine($"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }
  }
  [RelayCommand]
  void PrevForecastFromDb(object? obj)
  {
    if (obj is null) bpr.Click();
    if (!_store) return;

    _ = Task.Run(GetPastForecastFromDB).ContinueWith(_ =>
    {
      _.Result.a.ForEach(r => SctrPtTPFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
      _.Result.b.ForEach(r => SctrPtTPFVgn.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
      _.Result.c.ForEach(r => SctrPtTPFMis.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
      Model.InvalidatePlot(true); SmartAdd($"{(DateTime.Now - _now).TotalSeconds,6:N1}\t  Populated: From DB \t\t\t\t\t   \n");
      bpr.Tick();
    }, TaskScheduler.FromCurrentSynchronizationContext());
  }
  [RelayCommand]
  void PopulateScatModel(object? obj)
  {
    if (obj is null) bpr.Click();

    _ = Task.Run(async () => { var lst = await PlotViewModelHelpers.GetPast24hrFromEC(Cnst._Past24YYZ); return lst; }).ContinueWith(_ => { DrawPast24hrEC(Cnst.pearson, _.Result); _pastPea = _.Result; }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => { var lst = await PlotViewModelHelpers.GetPast24hrFromEC(Cnst._Past24YKZ); return lst; }).ContinueWith(_ => { DrawPast24hrEC(Cnst.batnvil, _.Result); _pastBvl = _.Result; }, TaskScheduler.FromCurrentSynchronizationContext());
     _ = Task.Run(async () => { var dta = await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._mississ); return dta; }).ContinueWith(_ => { DrawFore24hrEC(Cnst._mississ, _.Result); _foreMis = _.Result;         Model.Title += $"Miss \tat {(_foreMis?.currentConditions)?.dateTime[1].hour.Value}:{(_foreMis?.currentConditions)?.dateTime[1].minute}   {float.Parse(_foreMis?.currentConditions?.temperature?.Value??"-999F"),5:+##.#;-##.#;0}°   {float.Parse( _foreMis?.currentConditions?.windChill?.Value ?? _foreMis?.currentConditions?.temperature?.Value ?? "-999"),4:+##;-##;0}° {_foreMis?.currentConditions?.wind?.speed?.Value,5:N1}{(_foreMis?.currentConditions?.wind)?.speed.units} \n";    }, TaskScheduler.FromCurrentSynchronizationContext());

    _ = Task.Run(async () => { var dta = await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._vaughan); return dta; }).ContinueWith(_ => { DrawFore24hrEC(Cnst._vaughan, _.Result); _foreVgn = _.Result; }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => { var dta = await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._toronto); return dta; }).ContinueWith(_ => { DrawFore24hrEC(Cnst._toronto, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => { var dta = await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._torIsld); return dta; }).ContinueWith(_ => { DrawFore24hrEC(Cnst._torIsld, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());
    _ = Task.Run(async () => { var dta = await PlotViewModelHelpers.GetFore24hrFromEC(Cnst._markham); return dta; }).ContinueWith(_ => { DrawFore24hrEC(Cnst._markham, _.Result); }, TaskScheduler.FromCurrentSynchronizationContext());

    _ = Task.Run<object>(async () => await _opnwea.GetIt(_cfg["AppSecrets:MagicWeather"] ?? throw new ArgumentNullException(nameof(obj)), OpenWea.OpenWeatherCd.OneCallApi) ?? throw new ArgumentNullException(nameof(obj))).ContinueWith(async _ =>
    {
      oca = _.Result as RootobjectOneCallApi; ArgumentNullException.ThrowIfNull(oca); // PHC107

      //SmartAdd($"{(DateTime.Now-_now).TotalSeconds,5:N1}  {oca.current}";
     Model.Title /*= CurrentConditions*/ += $"OWA \tat {OpenWea.UnixToDt(oca.current.dt):HH:mm}   {oca.current.temp,5:+##.#;-##.#;0}°   {oca.current.feels_like,4:+##;-##;0}° {oca.current.wind_speed * _ms2kh / _wk,5:N1}k/h                                                                                                                                                                                                        \n";
      WindDirn = oca.current.wind_deg;
      WindVeloKmHr = oca.current.wind_speed * _ms2kh / _wk;
      WindGustKmHr = oca.current.wind_gust * _ms2kh / _wk;
      CurTempReal = $"{oca.current.temp:+#.#;-#.#;0}°";
      CurTempFeel = $"{oca.current.feels_like:+#;-#;0}°";
      CurWindKmHr = $"{WindVeloKmHr:N1}";

      OpnWeaIcom = $"http://openweathermap.org/img/wn/{oca.current.weather.First().icon}@2x.png";

      for (var i = 0; i < oca.daily.Length; i++) OpnWeaIcoA.Add($"http://openweathermap.org/img/wn/{oca.daily[i].weather[0].icon}@2x.png");

      oca.hourly.ToList().ForEach(x =>
      {
        //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(day0.dt)), day0.snow?._1h ?? 0, day0.snow?._1h ?? 0, _d3c)); // either null or 0 so far.
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp));
        OwaLoclFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.feels_like));
        OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pressure - 1030));
        OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _ms2kh));
        OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));

        var rad = Math.PI * x.wind_deg * 2 / 360;
        var dx = 0.10 * Math.Cos(rad);
        var dy = 10.0 * Math.Sin(rad);
        var tx = .002 * Math.Cos(rad + 90);
        var ty = 0.20 * Math.Sin(rad + 90);
        var sx = .002 * Math.Cos(rad - 90);
        var sy = 0.20 * Math.Sin(rad - 90);
        ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + tx, ty + (x.wind_speed * _ms2kh)));
        ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + dx, dy + (x.wind_speed * _ms2kh)));
        ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + sx, sy + (x.wind_speed * _ms2kh)));
      });

      var day0 = oca.daily.First();
      OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(day0.sunrise).AddDays(-1).ToOADate(), -000));
      OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(day0.sunrise).AddDays(-1).ToOADate(), +500));
      OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(day0.sunset).AddDays(-1).ToOADate(), +500));
      OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(day0.sunset).AddDays(-1).ToOADate(), -000));
      oca.daily.ToList().ForEach(x =>
      {
        OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunrise).ToOADate(), -000));
        OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunrise).ToOADate(), +500));
        OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunset).ToOADate(), +500));
        OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunset).ToOADate(), -000));
      });

      DrawSunSinosoid(day0);

      var d53 = await _opnwea.GetIt(_cfg["AppSecrets:MagicWeather"] ?? throw new ArgumentNullException(nameof(obj)), OpenWea.OpenWeatherCd.Frc5Day3Hr) as RootobjectFrc5Day3Hr; ArgumentNullException.ThrowIfNull(d53); // PHC107
      if (d53 != null)
      {
        DrawD53(d53);
        if (oca != null)
          DrawBothWhenReady(oca, d53);
      }

      await GetDays(2);

      Model.InvalidatePlot(true); SmartAdd($"{(DateTime.Now - _now).TotalSeconds,6:N1}\t  OWA  \n");

      await DelayedStoreToDbIf(50_000);

    }, TaskScheduler.FromCurrentSynchronizationContext());
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

      //if (Environment.UserDomainName != "RAZER1") try { _dbx.EnsureCreated22(); } catch (Exception ex) { WriteLine($"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }
      //WriteLine($"*** {_dbx.Database.GetConnectionString()}"); // 480ms

      var dbx = _dbh.WeatherxContext;

      var a = await dbx.PointFore.Where(r => r.SiteId == Cnst._phc && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync();
      var b = await dbx.PointFore.Where(r => r.SiteId == Cnst._vgn && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync();
      var c = await dbx.PointFore.Where(r => r.SiteId == Cnst._mis && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync();

      return (a, b, c);
    }
    catch (Exception ex) { WriteLine($"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); throw; }
    finally { _isDbBusy = false; }
  }
  void GetImprtandDatFromPearson(siteData? sitedata)
  {
    ArgumentNullException.ThrowIfNull(sitedata, $"@@@@@@@@@ {nameof(sitedata)}");

    if (double.TryParse(sitedata.almanac.temperature[0].Value, out var exmx)) { _extrMax = exmx; YAxsRMax = _vOffsetWas200 + (10 * (YAxisMax = exmx + _yHi)); }

    if (double.TryParse(sitedata.almanac.temperature[1].Value, out var exmn)) { _extrMin = exmn; YAxsRMin = _vOffsetWas200 + (10 * (YAxisMin = (Math.Floor(exmn / 10) * 10) - _yLo)); }

    if (double.TryParse(sitedata.almanac.temperature[2].Value, out var nrmx)) NormTMax = nrmx;
    if (double.TryParse(sitedata.almanac.temperature[3].Value, out var nrmn)) NormTMin = nrmn;

    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-2)), _extrMax));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(+8)), _extrMax));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(+8)), NormTMax));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), NormTMax));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), NormTMin));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(+8)), NormTMin));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(+8)), _extrMin));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-2)), _extrMin));

    EnvtCaIconM = $"https://weather.gc.ca/weathericons/{sitedata?.currentConditions?.iconCode?.Value ?? "5":0#}.gif"; // img1.Source = new BitmapImage(new Uri($"https://weather.gc.ca/weathericons/{(sitedata?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"));
  }
  async Task<bool> DelayedStoreToDbIf(int delayMs)
  {
    if (!_store) return false;

    await Task.Delay(delayMs); //hack: //todo: fix the db synch one day.

    try
    {
      await PlotViewModelHelpers.AddPast24hrToDB_EnvtCa(_dbh.WeatherxContext, Cnst.pearson, _pastPea);
      await PlotViewModelHelpers.AddPast24hrToDB_EnvtCa(_dbh.WeatherxContext, Cnst.batnvil, _pastBvl);
      await PlotViewModelHelpers.AddForecastToDB_EnvtCa(_dbh.WeatherxContext, Cnst._mis, _foreVgn);
      await PlotViewModelHelpers.AddForecastToDB_EnvtCa(_dbh.WeatherxContext, Cnst._vgn, _foreMis);
      await PlotViewModelHelpers.AddForecastToDB_OpnWea(_dbh.WeatherxContext, Cnst._phc, oca);

      SpeechSynth _synth = new(_cfg["AppSecrets:MagicSpeech"] ?? "Check cfg", true, CC.EnusAriaNeural.Voice);
      await _synth.SpeakProsodyAsync("All stored to DB.");

      SmartAdd($"{(DateTime.Now - _now).TotalSeconds,6:N1}\t  All stored to DB! \n");

      return true;
    }
    catch (Exception ex) { WriteLine($"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); throw; }
  }

  void DrawPast24hrEC(string site, List<MeteoDataMy> lst)
  {
    try
    {
      switch (site)
      {
        case Cnst.pearson: PlotViewModelHelpers.RefillPast24(ECaMissTemp, ECaBtvlWind, lst, _wk); break;
        case Cnst.batnvil: PlotViewModelHelpers.RefillPast24(ECaVghnTemp, ECaPearWind, lst, _wk); break;
        default: break;
      }

      //todo: OwaLoclPrsr.Clear();    dta.OrderBy(r => r.TakenAt).ToList().ForEach(x => OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), (10 * x.Pressure) - 1030)));

      Model.InvalidatePlot(true); SmartAdd($"{(DateTime.Now - _now).TotalSeconds,6:N1}\t  Envt CA  Past       \t{site}\t {YAxisMin}  {YAxisMax,-4}    {YAxsRMin,-4}  {YAxsRMax} \t   \n");    //await TickRepaintDelay();
    }
    catch (Exception ex) { WriteLine($"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }
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
        case Cnst._vaughan: PlotViewModelHelpers.RefillForeEnvtCa(ECaVghnTemp, sitedata); break;
        case Cnst._markham: PlotViewModelHelpers.RefillForeEnvtCa(ECaMrkhTemp, sitedata); break;
        default: break;
      }

      Model.InvalidatePlot(true); SmartAdd($"{(DateTime.Now - _now).TotalSeconds,6:N1}\t  Envt CA      Fore   \t{site.Substring(4, 3)}\t {YAxisMin}  {YAxisMax,-4}    {YAxsRMin,-4}  {YAxsRMax} \t   \n");
    }
    catch (Exception ex) { WriteLine($"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }
  }
  void DrawD53(RootobjectFrc5Day3Hr D53)
  {
    var h3 = 0; for (; h3 < OpenWea.UnixToDt(D53.list[0].dt).Hour / 3; h3++)
    {
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/01n.png", UriKind.Absolute)); //nogo: OpnWeaIco3[h3] = new BitmapImage(new Uri($"/Views/NoDataIndicator.bmp", UriKind.Absolute));
      OpnWeaTip3[h3] = $"{h3}";
    }

    D53.list.ToList().ForEach(r =>
    {
      //tmi: WriteLine($"});d53:  {r.dt_txt}    {UnixToDt(r.dt)}    {UnixToDt(r.dt):MMM dd  HH:mm}     {r.weather[0].description,-26}       {r.main.temp_min,6:N1} ÷ {r.main.temp_max,4:N1}°    pop{r.pop * 100,3:N0}%      {r}");
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{r.weather[0].icon}@2x.png"));
      OpnWeaTip3[h3] = $"{OpenWea.UnixToDt(r.dt):MMM d  H:mm} \n\n    {r.weather[0].description}   \n    {r.main.temp:N1}°    pop {r.pop * 100:N0}%";
      h3++;
    });
  }
  void DrawSunSinosoid(Daily x)
  {
    SunSinusoid.Clear();

    var t0 = OpenWea.UnixToDt(x.sunrise).ToOADate();
    var dh = 16 * Math.Cos(t0 * Math.PI * 2);

    FunctionSeries__(Math.Cos, t0 - 1.3, t0 + 7.4, .0125);
    void FunctionSeries__(Func<double, double> f, double x0, double x1, double dx)
    {
      for (var t = x0; t <= x1 + (dx * 0.5); t += dx)
        SunSinusoid.Add(new DataPoint(t, dh - (16 * f(t * Math.PI * 2))));
    }
  }
  void DrawBothWhenReady(RootobjectOneCallApi OCA, RootobjectFrc5Day3Hr D53)
  {
    D53.list.Where(d => d.dt > OCA.hourly.Max(d => d.dt)).ToList().ForEach(x =>
    {
      //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(day0.dt)), day0.snow?._3h ?? 0, day0.snow?._3h ?? 0, _d3c));
      OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));
      OwaLoclFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like));
      OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.pressure - 1030));
      OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust * _ms2kh));
      OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));

      var rad = Math.PI * x.wind.deg * 2 / 360;
      var dx = 0.1 * Math.Cos(rad);
      var dy = 1.0 * Math.Sin(rad);
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _ms2kh));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + dx, dy + (x.wind.speed * _ms2kh)));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _ms2kh));
    });

    OCA.daily.Where(d => d.dt > D53.list.Max(d => d.dt)).ToList().ForEach(x =>
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
  //static async Task TickRepaintDelay() { bpr.Tick(); await Task.Delay(_timeToPaintMS); }

  [Browsable(false)][ObservableProperty] PlotModel model = new() { TextColor = OxyColors.Magenta }; //void OnModelChanged() => PropertiesChanged();  void PropertiesChanged() => Model = ModelClearAdd("Prop Chgd");

  void ModelClearAdd(string note)
  {
    WriteLine($"::: {note}");
    //Model = new PlotModel(); // { Title = note, TextColor = OxyColors.OrangeRed };
    try
    {
      Model.Legends.Clear();
      Model.Legends.Add(new Legend { LegendTextColor = OxyColors.LightGray, LegendPosition = LegendPosition.LeftMiddle, LegendMargin = 10, LegendBackground = _111 });

      ReCreateAxises(note); // throws without

      Model.Series.Clear();
      Model.Series.Add(new AreaSeries { ItemsSource = OwaLoclSunT, Color = _330, StrokeThickness = 0.0, Title = "SunRS", YAxisKey = "yAxisR" });
      Model.Series.Add(new AreaSeries { ItemsSource = OwaLoclPopr, Color = _PoP, StrokeThickness = 0.0, Title = "PoPr", InterpolationAlgorithm = IA, YAxisKey = "yAxisR" });
      Model.Series.Add(new AreaSeries { ItemsSource = ECaBtvlWind, Color = _Phc, StrokeThickness = 0.5, Title = "Wind Btv", YAxisKey = "yAxisR", Fill = _550f });
      Model.Series.Add(new LineSeries { ItemsSource = ECaPearWind, Color = _Phc, StrokeThickness = 0.5, Title = "Wind Pea", YAxisKey = "yAxisR" });
      Model.Series.Add(new LineSeries { ItemsSource = SunSinusoid, Color = _cc0, StrokeThickness = 0.5, Title = "SunRS", YAxisKey = "yAxisL" });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclNowT, Color = _aaa, StrokeThickness = 1.0, Title = "owa T" });
      Model.Series.Add(new LineSeries { ItemsSource = ECaToroTemp, Color = _b80, StrokeThickness = 5.0, Title = "EC TO T", LineStyle = LineStyle.LongDash });
      Model.Series.Add(new LineSeries { ItemsSource = ECaMissTemp, Color = _Mis, StrokeThickness = 3.0, Title = "EC Mi T", LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = ECaTIslTemp, Color = _d00, StrokeThickness = 1.0, Title = "EC TI T", LineStyle = LineStyle.Dot });
      Model.Series.Add(new LineSeries { ItemsSource = ECaVghnTemp, Color = _Vgn, StrokeThickness = 3.0, Title = "EC VA T", LineStyle = LineStyle.Dash });
      Model.Series.Add(new LineSeries { ItemsSource = ECaMrkhTemp, Color = _Vgn, StrokeThickness = 2.0, Title = "EC MA T", LineStyle = LineStyle.Dash, InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclTemp, Color = _Phc, StrokeThickness = 1.5, Title = "owa Temp", InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclFeel, Color = _Phc, StrokeThickness = 0.5, Title = "owa Feel", InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclPrsr, Color = _Prs, StrokeThickness = 1.0, Title = "owa Prsr", InterpolationAlgorithm = IA, LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclGust, Color = _wnd, StrokeThickness = 0.5, Title = "owa Gust", InterpolationAlgorithm = IA, YAxisKey = "yAxisR" });
      Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFVgn, MarkerFill = OxyColors.Transparent, Title = "ECa _Vgn", MarkerType = MarkerType.Circle, MarkerStroke = _Vgn });
      Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFMis, MarkerFill = OxyColors.Transparent, Title = "ECa _Mis", MarkerType = MarkerType.Circle, MarkerStroke = _Mis });
      Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFPhc, MarkerFill = OxyColors.Transparent, Title = "OWA _Phc", MarkerType = MarkerType.Circle, MarkerStroke = _Phc, TrackerFormatString = "{}{0}&#xA;Time:   {2:HH:mm} &#xA;Temp:  {4:0.0}° " });
    }
    catch (Exception ex) { WriteLine($"■─■─■ {ex.Message} ■─■─■"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }

    Model.InvalidatePlot(true); //SmartAdd($"{(DateTime.Now - _now).TotalSeconds,6:N1}\t  Model re-freshed\t▓  {note,-26}  \t\t   \n";
  }
  void ReCreateAxises(string note)
  {
    var dddD = "                  ddd d";
    Model.Axes.Clear();
    Model.Axes.Add(new DateTimeAxis { Minimum = timeMin, Maximum = timeMax, MajorStep = 1, MinorStep = .250, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, StringFormat = dddD, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid, MajorGridlineColor = _mjg, MinorGridlineColor = _mng, MinorTickSize = 4, TicklineColor = _ccc, Position = AxisPosition.Top });
    Model.Axes.Add(new DateTimeAxis { Minimum = timeMin, Maximum = timeMax, MajorStep = 1, MinorStep = .250, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, StringFormat = dddD, Position = AxisPosition.Bottom });
    Model.Axes.Add(new LinearAxis { Minimum = YAxisMin, Maximum = YAxisMax, MajorStep = 010, MinorStep = 01, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = _mjg, Key = "yAxisL", Title = "Temp [°C]", MinorTickSize = 4, TicklineColor = _ccc });
    Model.Axes.Add(new LinearAxis { Minimum = YAxsRMin, Maximum = YAxsRMax, MajorStep = 100, MinorStep = 10, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, Position = AxisPosition.Right, MajorGridlineStyle = LineStyle.None, MajorGridlineColor = _mng, Key = "yAxisR", Title = "Wind k/h  PoP %" });

    Model.InvalidatePlot(true);
    SmartAdd($"{(DateTime.Now - _now).TotalSeconds,6:N1}\t  Axiss re-adjustd\t■  {note,-26}  \n");
  }
  void SmartAdd(string note)
  {
    SubHeader += note;
    var len = SubHeader.Length;
    var max = 260;
    if (len > max)
      SubHeader = $"   ...\t  {SubHeader.Substring(len - max, max)}";
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
    ECaBtvlWind.Clear();
    ECaPearWind.Clear();
    OwaLoclSunT.Clear();
    OwaLoclNowT.Clear();
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
  [ObservableProperty] double normTMin = -08;
  [ObservableProperty] double normTMax = +02;
  [ObservableProperty] double yAxisMin = -18; partial void OnYAxisMinChanged(double value) => ReCreateAxises("Y min");
  [ObservableProperty] double yAxisMax = +12; partial void OnYAxisMaxChanged(double value) => ReCreateAxises("Y max");
  [ObservableProperty] double yAxsRMax = +180; partial void OnYAxsRMaxChanged(double value) => ReCreateAxises("Y max R");
  [ObservableProperty] double yAxsRMin = -120; partial void OnYAxsRMinChanged(double value) => ReCreateAxises("Y min R");
  [ObservableProperty] double windGustKmHr;
  [ObservableProperty] IInterpolationAlgorithm iA = InterpolationAlgorithms.CatmullRomSpline; // the least vertical jumping beyond y value.

  RootobjectOneCallApi? oca;
  siteData? _foreVgn, _foreMis;
  List<MeteoDataMy>? _pastPea, _pastBvl;
  private bool _isDbBusy;
  #endregion
}