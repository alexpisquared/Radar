#define ObsCol // Go figure: ObsCol works, while array NOT! Just an interesting factoid.
namespace OpenWeaWpfApp;
public partial class PlotViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableValidator
{
  #region fields
  readonly int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600, _yHi = 2, _yLo = 13;
  readonly IConfigurationRoot _cfg;
  readonly WeatherxContext _dbx;
  readonly OpenWea _opnwea;
  int _days = 5; bool _busy;
  const int _maxIcons = 50, _timeToPaintMS = 88;
  double _extrMax = +20, _extrMin = -20;
  const string _toronto = "s0000458", _torIsld = "s0000785", _mississ = "s0000786", _vaughan = "s0000584", _markham = "s0000585", _richmhl = "s0000773", _newmark = "s0000582",
    _phc = "phc", _vgn = "vgn", _mis = "mis",
    _urlPast24hrYYZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=yyz", // Pearson
    _urlPast24hrYKZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=ykz"; // Buttonville
  OxyColor _550f = OxyColor.FromArgb(0x50, 0x50, 0x00, 0xff),
     _ooo = OxyColor.FromRgb(0x00, 0x00, 0x00),
     _330 = OxyColor.FromRgb(0x30, 0x30, 0x00),
     _111 = OxyColor.FromRgb(0x10, 0x10, 0x10),
     _333 = OxyColor.FromRgb(0x30, 0x30, 0x30),
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
  const float _wk = 10f, _kprsr = .01f;
  const float _kWind = 3.6f * _wk;

  [ObservableProperty] ObservableCollection<ScatterPoint> sctrPtTPFVgn = new();
  [ObservableProperty] ObservableCollection<ScatterPoint> sctrPtTPFPhc = new();
  [ObservableProperty] ObservableCollection<ScatterPoint> sctrPtTPFMis = new();
  [ObservableProperty] ObservableCollection<DataPoint> owaLoclTemp = new();
  [ObservableProperty] ObservableCollection<DataPoint> owaLoclFeel = new();
  [ObservableProperty] ObservableCollection<DataPoint> owaLoclPrsr = new();
  [ObservableProperty] ObservableCollection<DataPoint> owaLoclGust = new();
  [ObservableProperty] ObservableCollection<DataPoint> owaLoclGus_ = new();
  [ObservableProperty] ObservableCollection<DataPoint> eCaBtvlWind = new();
  [ObservableProperty] ObservableCollection<DataPoint> eCaPearWind = new();
  [ObservableProperty] ObservableCollection<DataPoint> owaLoclSunT = new();
  [ObservableProperty] ObservableCollection<DataPoint> owaLoclNowT = new();
  [ObservableProperty] ObservableCollection<DataPoint> owaLoclPopr = new();
  [ObservableProperty] ObservableCollection<DataPoint> eCaToroTemp = new();
  [ObservableProperty] ObservableCollection<DataPoint> eCaVghnTemp = new();
  [ObservableProperty] ObservableCollection<DataPoint> eCaMrkhTemp = new();
  [ObservableProperty] ObservableCollection<DataPoint> eCaMissTemp = new();
  [ObservableProperty] ObservableCollection<DataPoint> eCaTIslTemp = new();
  [ObservableProperty] double timeMin = DateTime.Today.ToOADate() - 1;
  [ObservableProperty] double timeMax = DateTime.Today.ToOADate() + 3;
  [ObservableProperty] string plotTitle;
  [ObservableProperty] string currentConditions;
  [ObservableProperty] string curTempReal;
  [ObservableProperty] string curTempFeel;
  [ObservableProperty] string curWindKmHr;
  [ObservableProperty] int windDirn;
  [ObservableProperty] float windVeloKmHr;
  [ObservableProperty] RootobjectOneCallApi? oCA = default!;
  [ObservableProperty] RootobjectFrc5Day3Hr? d53 = default!;
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

  public PlotViewModel(WeatherxContext weatherxContext, OpenWea openWea)
  {
    _cfg = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    _opnwea = openWea;
    _dbx = weatherxContext; // WriteLine($"*** {_dbx.Database.GetConnectionString()}"); // 480ms

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

    CreateModel("ctor");
  }

  [RelayCommand]
  public async Task<bool> PopulateAllAsync()
  {
    _busy = true;
    try
    {
      BprKernel32.StartFAF();

      //ClearData();
      await PrevForecastFromDbAsync();      
      //await PopulateEnvtCanaAsync();        
      //await PopulateScatModelAsync(_days);

      //CreateModel("Populated");

      BprKernel32.Finish();
    }
    catch (Exception ex)
    {
      WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@");
      if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
      return false;
    }
    finally { _busy = false; }

    return true;
  }
  [RelayCommand]
  void ClearData()
  {
    BprKernel32.ClickFAF();

    PlotTitle =
    CurrentConditions = "";
    WindDirn = 0;
    WindVeloKmHr = 0;
    OpnWeaIcom = "http://openweathermap.org/img/wn/01n@2x.png";
    ECaBtvlWind.Clear();
    OwaLoclPrsr.Clear();
    OwaLoclGust.Clear();
    OwaLoclTemp.Clear();
    OwaLoclFeel.Clear();
    OwaLoclFeel.Clear();
    OwaLoclSunT.Clear();
    OwaLoclPopr.Clear();
  }
  [RelayCommand]
  async Task PrevForecastFromDbAsync()
  {
    BprKernel32.ClickFAF();

    if (_cfg["StoreData"] != "Yes")
      return;

    var now = DateTime.Now;
    var ytd = now.AddHours(-24);
    var dby = now.AddHours(-48);

    if (Environment.UserDomainName != "RAZER1") try { _dbx.EnsureCreated22(); } catch (Exception ex) { WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }

    //WriteLine($"*** {_dbx.Database.GetConnectionString()}"); // 480ms

    (await _dbx.PointFore.Where(r => r.SiteId == _phc && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
    (await _dbx.PointFore.Where(r => r.SiteId == _vgn && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFVgn.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
    (await _dbx.PointFore.Where(r => r.SiteId == _mis && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFMis.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
  }
  [RelayCommand]
  async Task PopulateEnvtCanaAsync()
  {
    BprKernel32.ClickFAF();
    Past24hrHAP p24 = new();
    var bvl = await p24.GetIt(_urlPast24hrYKZ);
    var pea = await p24.GetIt(_urlPast24hrYYZ);

    if (_cfg["StoreData"] == "Yes")
    {
      await PlotViewModelHelpers.AddPastDataToDB_EnvtCa(_dbx, "bvl", bvl);
      await PlotViewModelHelpers.AddPastDataToDB_EnvtCa(_dbx, "pea", pea);
    }

    PlotViewModelHelpers.RefillPast24(ECaVghnTemp, ECaPearWind, bvl, _wk);
    PlotViewModelHelpers.RefillPast24(ECaMissTemp, ECaBtvlWind, pea, _wk);

    OwaLoclPrsr.Clear(); pea.OrderBy(r => r.TakenAt).ToList().ForEach(x => OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), (10 * x.Pressure) - 1030)));

    await TickRepaintDelay();

    var sitedataMiss = await OpenWea.GetEnvtCa(_mississ);
    var sitedataVghn = await OpenWea.GetEnvtCa(_vaughan);

    if (_cfg["StoreData"] == "Yes")
    {
      await PlotViewModelHelpers.AddForeDataToDB_EnvtCa(_dbx, "mis", sitedataMiss);
      await PlotViewModelHelpers.AddForeDataToDB_EnvtCa(_dbx, "vgn", sitedataVghn);
    }

    PlotViewModelHelpers.RefillForeEnvtCa(ECaToroTemp, await OpenWea.GetEnvtCa(_toronto));
    PlotViewModelHelpers.RefillForeEnvtCa(ECaTIslTemp, await OpenWea.GetEnvtCa(_torIsld));    //refill(ECaTIslTemp, await _opnwea.GetEnvtCa(_newmark));
    PlotViewModelHelpers.RefillForeEnvtCa(ECaMissTemp, sitedataMiss);
    PlotViewModelHelpers.RefillForeEnvtCa(ECaVghnTemp, sitedataVghn);
    PlotViewModelHelpers.RefillForeEnvtCa(ECaMrkhTemp, await OpenWea.GetEnvtCa(_markham));

    await TickRepaintDelay();

    ArgumentNullException.ThrowIfNull(sitedataMiss, $"@@@@@@@@@ {nameof(sitedataMiss)}");
    SubHeader += $"{sitedataMiss.currentConditions.wind.speed}\n";

    if (double.TryParse(sitedataMiss.almanac.temperature[0].Value, out var d)) { YAxsRMax = 200 + (10 * (YAxisMax = d + _yHi)); _extrMax = d; }

    if (double.TryParse(sitedataMiss.almanac.temperature[1].Value, out /**/d)) { YAxsRMin = 200 + (10 * (YAxisMin = (Math.Floor(d / 10) * 10) - _yLo)); _extrMin = d; }

    if (double.TryParse(sitedataMiss.almanac.temperature[2].Value, out d)) NormTMax = d;
    if (double.TryParse(sitedataMiss.almanac.temperature[3].Value, out d)) NormTMin = d;

    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-2)), _extrMax));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(+8)), _extrMax));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(+8)), NormTMax));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), NormTMax));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), NormTMin));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(+8)), NormTMin));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(+8)), _extrMin));
    OwaLoclNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-2)), _extrMin));

    EnvtCaIconM = $"https://weather.gc.ca/weathericons/{sitedataMiss?.currentConditions?.iconCode?.Value ?? "5":0#}.gif"; // img1.Source = new BitmapImage(new Uri($"https://weather.gc.ca/weathericons/{(sitedata?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"));
    EnvtCaIconV = $"https://weather.gc.ca/weathericons/{sitedataVghn?.currentConditions?.iconCode?.Value ?? "5":0#}.gif"; // img1.Source = new BitmapImage(new Uri($"https://weather.gc.ca/weathericons/{(sitedata?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"));
  
    BprKernel32.ClickFAF();
  }
  [RelayCommand]
  async Task PopulateScatModelAsync()
  {
    BprKernel32.ClickFAF();

    OCA = await _opnwea.GetIt(_cfg["AppSecrets:MagicNumber"], OpenWea.OpenWeatherCd.OneCallApi) as RootobjectOneCallApi; ArgumentNullException.ThrowIfNull(OCA); // PHC107
    D53 = await _opnwea.GetIt(_cfg["AppSecrets:MagicNumber"], OpenWea.OpenWeatherCd.Frc5Day3Hr) as RootobjectFrc5Day3Hr; ArgumentNullException.ThrowIfNull(D53); // PHC107

    SubHeader += $"{OCA.current}";
    PlotTitle = CurrentConditions = $"{OpenWea.UnixToDt(OCA.current.dt):HH:mm:ss}   {OCA.current.temp,5:N1}°   {OCA.current.feels_like,4:N0}°  {OCA.current.wind_speed * _kWind:N1}k/h";
    WindDirn = OCA.current.wind_deg;
    WindVeloKmHr = OCA.current.wind_speed * _kWind / _wk;
    WindGustKmHr = OCA.current.wind_gust * _kWind / _wk;
    CurTempReal = $"{OCA.current.temp:+#.#;-#.#;0}°";
    CurTempFeel = $"{OCA.current.feels_like:+#;-#;0}°";
    CurWindKmHr = $"{WindVeloKmHr:N1}";

    OpnWeaIcom = $"http://openweathermap.org/img/wn/{OCA.current.weather.First().icon}@2x.png";

    for (var i = 0; i < OCA.daily.Length; i++) OpnWeaIcoA.Add($"http://openweathermap.org/img/wn/{OCA.daily[i].weather[0].icon}@2x.png");

#if NoGo // XAML is noticing the missing array indexes:
    OpnWeaIco3.Clear();
    OpnWeaTip3.Clear();

    for (var h3 = 0; h3 < UnixToDt(D53.list[0].dt).Hour / 3; h3++)
    {
      OpnWeaIco3.Add(new BitmapImage(new Uri($"http://openweathermap.org/img/wn/01n.png")));
      OpnWeaTip3.Add($"{h3}");
    }
    D53.list.ToList().ForEach(r =>
    {
      WriteLine($"D53:  {r.dt_txt}    {UnixToDt(r.dt)}    {UnixToDt(r.dt):MMM d  H:mm}     {r.weather[0].description,-26}       {r.main.temp_min} - {r.main.temp_max}°    pop {r.pop * 100:N0}      {r}");
      OpnWeaIco3.Add(new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{r.weather[0].icon}@2x.png")));
      OpnWeaTip3.Add($"{UnixToDt(r.dt):MMM d  H:mm} \n\n    {r.weather[0].description}   \n    {r.main.temp_min} - {r.main.temp_max}°    pop {r.pop * 100:N0}");
    });
#else
    var h3 = 0;
    for (; h3 < OpenWea.UnixToDt(D53.list[0].dt).Hour / 3; h3++)
    {
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/01n.png", UriKind.Absolute)); //nogo: OpnWeaIco3[h3] = new BitmapImage(new Uri($"/Views/NoDataIndicator.bmp", UriKind.Absolute));
      OpnWeaTip3[h3] = $"{h3}";
    }

    D53.list.ToList().ForEach(r =>
    {
      //tmi: WriteLine($"});D53:  {r.dt_txt}    {UnixToDt(r.dt)}    {UnixToDt(r.dt):MMM dd  HH:mm}     {r.weather[0].description,-26}       {r.main.temp_min,6:N1} ÷ {r.main.temp_max,4:N1}°    pop{r.pop * 100,3:N0}%      {r}");
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{r.weather[0].icon}@2x.png"));
      OpnWeaTip3[h3] = $"{OpenWea.UnixToDt(r.dt):MMM d  H:mm} \n\n    {r.weather[0].description}   \n    {r.main.temp:N1}°    pop {r.pop * 100:N0}%";
      h3++;
    });
#endif

    const int id = 2;
    await SetMaxX(id);
    TimeMin = DateTime.Today.AddDays(-1).ToOADate(); // == DateTimeAxis.ToDouble(DateTime.Today.AddDays(-1));
    TimeMax = DateTime.Today.AddDays(id).ToOADate(); // DateTimeAxis.ToDouble(_days == 5 ? UnixToDt(OCA.daily.Max(d => d.dt) + 12 * 3600) : DateTime.Today.AddDays(_days));
    var valueMax = _extrMax; // OCA.daily.Max(r => r.temp.max);
    var valueMin = _extrMin; // OCA.daily.Min(r => r.temp.min);

    if (_cfg["StoreData"] == "Yes") //if (_cfg["StoreData"] == "Yes") 
      await PlotViewModelHelpers.AddForeDataToDB_OpnWea(_dbx, "phc", OCA);

    OCA.hourly.ToList().ForEach(x =>
    {
      //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._1h ?? 0, x.snow?._1h ?? 0, _d3c)); // either null or 0 so far.
      OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp));
      OwaLoclFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.feels_like));
      OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pressure - 1030));
      OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _kWind));
      OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));

      var rad = Math.PI * x.wind_deg * 2 / 360;
      var dx = 0.10 * Math.Cos(rad);
      var dy = 10.0 * Math.Sin(rad);
      var tx = .002 * Math.Cos(rad + 90);
      var ty = 0.20 * Math.Sin(rad + 90);
      var sx = .002 * Math.Cos(rad - 90);
      var sy = 0.20 * Math.Sin(rad - 90);
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + tx, ty + (x.wind_speed * _kWind)));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + dx, dy + (x.wind_speed * _kWind)));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + sx, sy + (x.wind_speed * _kWind)));
    });

    await TickRepaintDelay();

    D53.list.Where(d => d.dt > OCA.hourly.Max(d => d.dt)).ToList().ForEach(x =>
    {
      //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._3h ?? 0, x.snow?._3h ?? 0, _d3c));
      OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));
      OwaLoclFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like));
      OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.pressure - 1030));
      OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust * _kWind));
      OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));

      var rad = Math.PI * x.wind.deg * 2 / 360;
      var dx = 0.1 * Math.Cos(rad);
      var dy = 1.0 * Math.Sin(rad);
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _kWind));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + dx, dy + (x.wind.speed * _kWind)));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _kWind));
    });

    var x = OCA.daily.First();
    OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunrise).AddDays(-1).ToOADate(), valueMin));
    OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunrise).AddDays(-1).ToOADate(), valueMax));
    OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunset).AddDays(-1).ToOADate(), valueMax));
    OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunset).AddDays(-1).ToOADate(), valueMin));
    OCA.daily.ToList().ForEach(x =>
    {
      OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunrise).ToOADate(), valueMin));
      OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunrise).ToOADate(), valueMax));
      OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunset).ToOADate(), valueMax));
      OwaLoclSunT.Add(new DataPoint(OpenWea.UnixToDt(x.sunset).ToOADate(), valueMin));
    });

    OwaLoclGus_.Clear();

    var t0 = OpenWea.UnixToDt(x.sunrise).ToOADate();
    var dh = 16 * Math.Cos(t0 * Math.PI * 2);

    FunctionSeries__(Math.Cos, t0 - 1.3, t0 + 7.4, .0125);
    void FunctionSeries__(Func<double, double> f, double x0, double x1, double dx)
    {
      for (var t = x0; t <= x1 + (dx * 0.5); t += dx)
        OwaLoclGus_.Add(new DataPoint(t, dh - (16 * f(t * Math.PI * 2))));
    }

    OCA.daily
      .Where(d => d.dt > D53.list.Max(d => d.dt))
      .ToList().ForEach(x =>
      {
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.temp.morn));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.temp.day));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.temp.eve));
        OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.temp.night));

        OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pressure - 1030));
        OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _kWind));
        OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 100));
      });

    //OCA.daily.ToList().ForEach(x =>
    //{
    //  SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), size: 3, y: x.temp.morn));
    //  SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), size: 5, y: x.temp.day));
    //  SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), size: 7, y: x.temp.eve));
    //  SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), size: 9, y: x.temp.night));
    //});
  
    BprKernel32.ClickFAF();
  }

  static async Task TickRepaintDelay() { BprKernel32.TickFAF(); await Task.Delay(_timeToPaintMS); }
  void CopyOpenWeaToOwaLoclLists(RootobjectOneCallApi? ocv, ObservableCollection<DataPoint> pointsTemp, ObservableCollection<DataPoint> pointsFeel)
  {
    ArgumentNullException.ThrowIfNull(ocv);

    ocv.hourly.ToList().ForEach(x =>
    {
      pointsTemp.Add(new(x.dt, x.temp));
      pointsFeel.Add(new(x.dt, x.feels_like));
    });

    ocv.daily.Where(d => d.dt > ocv.hourly.Max(d => d.dt)).ToList().ForEach(x =>
    {
      pointsTemp.Add(new(x.dt + _m, x.temp.morn));
      pointsTemp.Add(new(x.dt + _d, x.temp.day));
      pointsTemp.Add(new(x.dt + _e, x.temp.eve));
      pointsTemp.Add(new(x.dt + _n, x.temp.night));
      pointsFeel.Add(new(x.dt + _m, x.feels_like.morn));
      pointsFeel.Add(new(x.dt + _d, x.feels_like.day));
      pointsFeel.Add(new(x.dt + _e, x.feels_like.eve));
      pointsFeel.Add(new(x.dt + _n, x.feels_like.night));
    });
  }
  IRelayCommand? _cd; public IRelayCommand GetDaysComman_ => _cd ??= new RelayCommand(GetDays); void GetDays() { }
  IRelayCommand? _gs; public IRelayCommand GetDaysCommand => _gs ??= new AsyncRelayCommand<object>(SetMaxX, (days) => !_busy); async Task SetMaxX(object? days_)
  {
    if (OCA is not null && int.TryParse(days_?.ToString(), out var days))
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

    await Task.Yield();// PopulateAllAsync((int?)_days ?? 5);
  }

  [Browsable(false)][ObservableProperty] PlotModel model; //void OnModelChanged() => PropertiesChanged();  void PropertiesChanged() => Model = CreateModel("Prop Chgd");

  PlotModel CreateModel(string note)
  {
    string dddD = "                  ddd d";
    WriteLine($"::: {note}");
    Model = new PlotModel(); // { Title = note, TextColor = OxyColors.OrangeRed };
    try
    {
      //Model.Legends.Clear();
      Model.Legends.Add(new Legend { LegendTextColor = OxyColors.LightGray, LegendPosition = LegendPosition.LeftMiddle, LegendMargin = 10, LegendBackground=_111 });

      //Model.Axes.Clear();
      Model.Axes.Add(new DateTimeAxis { Minimum = timeMin, Maximum = timeMax, MajorStep = 1, MinorStep = .25, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, StringFormat = dddD, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid, MajorGridlineColor = _ooo, MinorGridlineColor = _333, MinorTickSize = 4, TicklineColor = _ccc, Position = AxisPosition.Top });
      Model.Axes.Add(new DateTimeAxis { Minimum = timeMin, Maximum = timeMax, MajorStep = 1, MinorStep = .25, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, StringFormat = dddD, Position = AxisPosition.Bottom });
      Model.Axes.Add(new LinearAxis { Minimum = YAxisMin, Maximum = YAxisMax, MajorStep = 010, MinorStep = 01, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = _ooo, Key = "yAxisL", Title = "Temp [°C]", MinorTickSize = 4, TicklineColor = _ccc });
      Model.Axes.Add(new LinearAxis { Minimum = YAxsRMin, Maximum = YAxsRMax, MajorStep = 100, MinorStep = 10, TextColor = _eee, TitleColor = _eee, IsZoomEnabled = false, IsPanEnabled = false, Position = AxisPosition.Right, MajorGridlineStyle = LineStyle.None, MajorGridlineColor = _ooo, Key = "yAxisR", Title = "Wind k/h  PoP %" });

      //Model.Series.Clear();
      Model.Series.Add(new AreaSeries { ItemsSource = OwaLoclSunT, Color = _330, StrokeThickness = 0.0, Title = "SunRS" });
      Model.Series.Add(new AreaSeries { ItemsSource = ECaBtvlWind, Color = _Phc, StrokeThickness = 0.5, Title = "Wind EC Owa", YAxisKey = "yAxisR", Fill = _550f });
      Model.Series.Add(new AreaSeries { ItemsSource = OwaLoclPopr, Color = _PoP, StrokeThickness = 0.0, Title = "PoPr", InterpolationAlgorithm = IA, YAxisKey = "yAxisR" });

      Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFVgn, MarkerFill = OxyColors.Transparent, Title = "ECa _Vgn", MarkerType = MarkerType.Circle, MarkerStroke = _Vgn });
      Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFMis, MarkerFill = OxyColors.Transparent, Title = "ECa _Mis", MarkerType = MarkerType.Circle, MarkerStroke = _Mis });
      Model.Series.Add(new ScatterSeries { ItemsSource = SctrPtTPFPhc, MarkerFill = OxyColors.Transparent, Title = "OWA _Phc", MarkerType = MarkerType.Circle, MarkerStroke = _Phc, TrackerFormatString = "{}{0}&#xA;Time:   {2:HH:mm} &#xA;Temp:  {4:0.0}° " });

      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclGus_, Color = _cc0, StrokeThickness = 0.5, Title = "SunRS", YAxisKey = "yAxisL" });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclNowT, Color = _aaa, StrokeThickness = 1.0, Title = "owa T" });
      Model.Series.Add(new LineSeries { ItemsSource = ECaToroTemp, Color = _b80, StrokeThickness = 5.0, Title = "EC TO T", LineStyle = LineStyle.LongDash });
      Model.Series.Add(new LineSeries { ItemsSource = ECaMissTemp, Color = _Mis, StrokeThickness = 3.0, Title = "EC Mi T", LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = ECaTIslTemp, Color = _d00, StrokeThickness = 1.0, Title = "EC TI T", LineStyle = LineStyle.Dot });
      Model.Series.Add(new LineSeries { ItemsSource = ECaVghnTemp, Color = _Vgn, StrokeThickness = 3.0, Title = "EC VA T", LineStyle = LineStyle.Dash });
      Model.Series.Add(new LineSeries { ItemsSource = ECaMrkhTemp, Color = _Vgn, StrokeThickness = 2.0, Title = "EC MA T", LineStyle = LineStyle.Dash, InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclTemp, Color = _Phc, StrokeThickness = 1.5, Title = "owa Temp", InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclFeel, Color = _Phc, StrokeThickness = 0.5, Title = "owa Feel", InterpolationAlgorithm = IA });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclPrsr, Color = _Prs, StrokeThickness = 1.0, Title = "owa Prsr", InterpolationAlgorithm = IA, LineStyle = LineStyle.LongDashDotDot });
      Model.Series.Add(new LineSeries { ItemsSource = ECaPearWind, Color = _Phc, StrokeThickness = 0.5, Title = "Wind EC Pea", YAxisKey = "yAxisR" });
      Model.Series.Add(new LineSeries { ItemsSource = OwaLoclGust, Color = _wnd, StrokeThickness = 0.5, Title = "owa Gust", InterpolationAlgorithm = IA, YAxisKey = "yAxisR" });
    }
    catch (Exception ex) { WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@"); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification); }

    return Model;
  }
  internal void ClearPlot()
  {
    Model.Legends.Clear();
    Model.Axes.Clear();
    Model.Series.Clear();
  }

  [RelayCommand] void Invalidate(object updateData) { BprKernel32.Tick(); Model?.InvalidatePlot(updateData?.ToString() == "true"); }
  [RelayCommand] void CreateMdl(object note) { BprKernel32.Tick(); CreateModel(note?.ToString() ?? "000"); }

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
  [ObservableProperty] string subHeader = "Loading...";
  [ObservableProperty] double yAxisMin = -18;
  [ObservableProperty] double yAxisMax = +12;
  [ObservableProperty] double normTMin = -08;
  [ObservableProperty] double normTMax = +02;
  [ObservableProperty] double yAxsRMax = +180;
  [ObservableProperty] double yAxsRMin = -120;
  [ObservableProperty] double windGustKmHr;
  [ObservableProperty] IInterpolationAlgorithm iA = InterpolationAlgorithms.CatmullRomSpline; // the least vertical jumping beyond y value.
  #endregion
}