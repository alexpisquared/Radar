#define ObsCol // Go figure: ObsCol works, while array NOT! Just an interesting factoid.
namespace OpenWeaWpfApp;
[Obsolete("Hello!?!?!?", false)]
public partial class MainPlotOldVM : ObservableValidator
{
  readonly IConfigurationRoot _cfg;
  readonly OpenWea _opnwea;
  readonly ILogger _lgr;
  readonly SpeechSynth _synth;
  readonly int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600, _yHi = 2, _yLo = 13;
  bool _busy;
  const int _maxIcons = 50, _timeToPaintMS = 88;
  readonly WeatherxContext _dbx;
  static readonly Bpr bpr = new();
  double _extrMax = +20, _extrMin = -20;
  const string _toronto = "s0000458", _torIsld = "s0000785", _mississ = "s0000786", _vaughan = "s0000584", _markham = "s0000585", _richmhl = "s0000773", _newmark = "s0000582",
    _phc = "phc", _vgn = "vgn", _mis = "mis",
    _urlPast24hrYYZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=yyz", // Pearson
    _urlPast24hrYKZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=ykz"; // Buttonville

  public MainPlotOldVM(WeatherxContext weatherxContext, OpenWea openWea, ILogger lgr, SpeechSynth synth)
  {
                                 //_cfg = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets min usersecrets 
    _lgr = lgr;
    this._synth = synth;
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

    _lgr.LogInformation("▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀");
  }
  public async Task<bool> PopulateAsync(int days = 5)
  {
    _busy = true;
    try
    {
      Clear();                              /**/            await Tick();
      await PrevForecastFromDB();           /**/ await Tick();
      await PopulateEnvtCanaAsync();        /**/ await Tick();
      await PopulateScatModelAsync(days);   /**/ await Tick();

      _synth.SpeakFAF("All stored to DB.");
    }
    catch (Exception ex)
    {
      WriteLine($"■─■─■ {ex.Message} \n\t {ex} ■─■─■");
      if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
      return false;
    }
    finally { _busy = false; }

    return true;
  }

  static async Task Tick() { bpr.Tick(); await Task.Delay(_timeToPaintMS); }

  async Task PrevForecastFromDB()
  {
    if (_cfg["StoreDataToDB"] != "Yes") //if (Environment.MachineName != "LR6WB43X")
      return;

    var now = DateTime.Now;
    var ytd = now.AddHours(-24);
    var dby = now.AddHours(-48);

    if (Environment.UserDomainName != "RAZER1")
    {
      try
      {
        _dbx.EnsureCreated22();
      }
      catch (Exception ex)
      {
        WriteLine($"■─■─■ {ex.Message} \n\t {ex} ■─■─■");
        if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
      }
    }

    //WriteLine($"*** {_dbx.Database.GetConnectionString()}"); // 480ms

    (await _dbx.PointFore.Where(r => r.SiteId == _phc && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
    (await _dbx.PointFore.Where(r => r.SiteId == _vgn && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFVgn.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
    (await _dbx.PointFore.Where(r => r.SiteId == _mis && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFMis.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + (.25 * (r.ForecastedFor - r.ForecastedAt).TotalHours), y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
  }
  async Task PopulateEnvtCanaAsync()
  {
    Past24hrHAP p24 = new();
    var bvl = await p24.GetIt(_urlPast24hrYKZ);
    var pea = await p24.GetIt(_urlPast24hrYYZ);

    if (_cfg["StoreDataToDB"] == "Yes") //if (Environment.MachineName != "LR6WB43X")         "StoreDataToDB": "No",
    {
      await AddPastDataToDB_EnvtCa("bvl", bvl);
      await AddPastDataToDB_EnvtCa("pea", pea);
    }

    RefillPast24(ECaVghnTemp, ECaPearWind, bvl);
    RefillPast24(ECaMissTemp, ECaBtvlWind, pea);

    OwaLoclPrsr.Clear(); pea.OrderBy(r => r.TakenAt).ToList().ForEach(x => OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), (10 * x.Pressure) - 1030)));

    await Tick();

    var sitedataMiss = await OpenWea.GetEnvtCa(_mississ);
    var sitedataVghn = await OpenWea.GetEnvtCa(_vaughan);

    if (_cfg["StoreDataToDB"] == "Yes") //if (Environment.MachineName != "LR6WB43X")
    {
      await AddForeDataToDB_EnvtCa("mis", sitedataMiss);
      await AddForeDataToDB_EnvtCa("vgn", sitedataVghn);
    }

    RefillForeEnvtCa(ECaToroTemp, await OpenWea.GetEnvtCa(_toronto));
    RefillForeEnvtCa(ECaTIslTemp, await OpenWea.GetEnvtCa(_torIsld));    //refill(ECaTIslTemp, await _opnwea.GetEnvtCa(_newmark));
    RefillForeEnvtCa(ECaMissTemp, sitedataMiss);
    RefillForeEnvtCa(ECaVghnTemp, sitedataVghn);
    RefillForeEnvtCa(ECaMrkhTemp, await OpenWea.GetEnvtCa(_markham));

    await Tick();

    ArgumentNullException.ThrowIfNull(sitedataMiss, $"@@@@@@@@@ {nameof(sitedataMiss)}");
    SubHeader += $"{sitedataMiss.currentConditions.wind.speed}\n";

    if (double.TryParse(sitedataMiss.almanac.temperature[0].Value, out var d)) { YAxiXMax = 200 + (10 * (YAxisMax = d + _yHi)); _extrMax = d; }

    if (double.TryParse(sitedataMiss.almanac.temperature[1].Value, out /**/d)) { YAxiXMin = 200 + (10 * (YAxisMin = (Math.Floor(d / 10) * 10) - _yLo)); _extrMin = d; }

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
  }
  async Task AddPastDataToDB_EnvtCa(string siteId, List<MeteoDataMy> sitePast, string srcId = "eca", string measureId = "tar")
  {
    ArgumentNullException.ThrowIfNull(sitePast, $"@@@@@@@@@ {nameof(sitePast)}");
    var now = DateTime.Now;

    //var connectionString = _cfg.GetConnectionString("Exprs");
    //WeatherxContextFactory dbf = new(connectionString);
    //using WeatherxContext _dbx = dbf.CreateDbContext();

    foreach (var f in sitePast) //sitePast.hourlyPastcastGroup.hourlyPastcast.ToList().ForEach(async f =>
    {
      if (await _dbx.PointReal.AnyAsync(d =>
          d.SrcId == srcId &&
          d.SiteId == siteId &&
          d.MeasureId == measureId &&
          d.MeasureTime == f.TakenAt) == false)
      {
        _ = _dbx.PointReal.Add(new PointReal
        {
          SrcId = srcId,
          SiteId = siteId,
          MeasureId = measureId,
          MeasureValue = f.TempAir,
          MeasureTime = f.TakenAt,
          Note64 = "[early runs]",
          CreatedAt = now
        });
      }
    };

    WriteLine($"PP{await _dbx.SaveChangesAsync()}");
  }
  async Task AddForeDataToDB_EnvtCa(string siteId, siteData? siteFore, string srcId = "eca", string measureId = "tar")
  {
    ArgumentNullException.ThrowIfNull(siteFore, $"@@@@@@@@@ {nameof(siteFore)}");
    var now = DateTime.Now;

    //var connectionString = _cfg.GetConnectionString("Exprs");
    //WeatherxContextFactory dbf = new(connectionString);
    //using WeatherxContext _dbx = dbf.CreateDbContext();

    var forecastedAt = EnvtCaDate(siteFore.currentConditions.dateTime[1]);

    foreach (var f in siteFore.hourlyForecastGroup.hourlyForecast.ToList()) //siteFore.hourlyForecastGroup.hourlyForecast.ToList().ForEach(async f =>
    {
      var forecastedFor = EnvtCaDate(f.dateTimeUTC);
      var val = double.Parse(f.temperature.Value);

      if (await _dbx.PointFore.AnyAsync(d =>
          d.SrcId == srcId &&
          d.SiteId == siteId &&
          d.MeasureId == measureId &&
          d.MeasureValue == val && // _store only changes in recalculation results
          d.ForecastedFor == forecastedFor) == false)
      {
        _ = _dbx.PointFore.Add(new PointFore
        {
          SrcId = srcId,
          SiteId = siteId,
          MeasureId = measureId,
          MeasureValue = val,
          ForecastedFor = forecastedFor,
          ForecastedAt = forecastedAt,
          Note64 = "[early runs]",
          CreatedAt = now
        });
      }
    };

    WriteLine($"PP{await _dbx.SaveChangesAsync()}");
  }
  async Task AddForeDataToDB_OpnWea(string siteId, RootobjectOneCallApi? siteFore, string srcId = "owa", string measureId = "tar")
  {
    ArgumentNullException.ThrowIfNull(siteFore, $"@@@@@@@@@ {nameof(siteFore)}");
    var now = DateTime.Now;

    //var connectionString = _cfg.GetConnectionString("Exprs");
    //WeatherxContextFactory dbf = new(connectionString);
    //using WeatherxContext _dbx = dbf.CreateDbContext();

    var forecastedAt = OpenWea.UnixToDt(siteFore.current.dt);

    foreach (var f in siteFore.hourly.ToList()) //siteFore.hourlyForecastGroup.hourlyForecast.ToList().ForEach(async f =>
    {
      var forecastedFor = OpenWea.UnixToDt(f.dt);

      if (await _dbx.PointFore.AnyAsync(d =>
          d.SrcId == srcId &&
          d.SiteId == siteId &&
          d.MeasureValue == f.temp && // _store only changes in recalculation results
          d.MeasureId == measureId &&
          d.ForecastedFor == forecastedFor) == false)
      {
        _ = _dbx.PointFore.Add(new PointFore
        {
          SrcId = srcId,
          SiteId = siteId,
          MeasureId = measureId,
          MeasureValue = f.temp,
          ForecastedFor = forecastedFor,
          ForecastedAt = forecastedAt,
          Note64 = "[early runs]",
          CreatedAt = now
        });
      }
    };

    WriteLine($"PP{await _dbx.SaveChangesAsync()}");
  }

  static DateTimeOffset EnvtCaDate(string yyyyMMddHHmm)
  {
    if (!DateTime.TryParseExact(yyyyMMddHHmm, new string[] { "yyyyMMddHHmm", "yyyyMMddHHmmss" }, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var utc)) //var lcl = DateTime.ParseExact(yyyyMMddHHmm, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToLocalTime(); //tu: UTC to Local time.
      throw new ArgumentOutOfRangeException(nameof(yyyyMMddHHmm), yyyyMMddHHmm, "■─■─■ Can you imagine?!?!?!");

    return utc.ToLocalTime(); //tu: UTC to Local time.
  }
  static DateTimeOffset EnvtCaDate(dateStampType dateStampType)
  {
    if (!DateTimeOffset.TryParseExact($"{dateStampType.year}-{dateStampType.month.Value}-{dateStampType.day.Value} {dateStampType.hour.Value}:{dateStampType.minute}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
      throw new ArgumentException(nameof(dateStampNameType));

    return dt;
  }

  static void RefillPast24(ObservableCollection<DataPoint> temps, ObservableCollection<DataPoint> winds, List<MeteoDataMy>? siteDt)
  {
    ArgumentNullException.ThrowIfNull(siteDt, $"@@@@@@@@@ {nameof(siteDt)}");

    temps.Clear();
    winds.Clear();
    siteDt.OrderBy(x => x.TakenAt).ToList().ForEach(x =>
      {
        temps.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), x.TempAir));
        winds.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), x.WindKmH * _wk));
      });
  }
  static void RefillForeEnvtCa(ObservableCollection<DataPoint> points, siteData? siteDt)
  {
    ArgumentNullException.ThrowIfNull(siteDt, $"@@@@@@@@@ {nameof(siteDt)}");

    //points.ClearData();
    siteDt.hourlyForecastGroup.hourlyForecast.ToList().ForEach(x => points.Add(new DataPoint(DateTimeAxis.ToDouble(EnvtCaDate(x.dateTimeUTC).DateTime), double.Parse(x.temperature.Value))));
  }

  async Task PopulateScatModelAsync(int days = 5)
  {
    var mgk = _cfg["AppSecrets:MagicWeather"] ?? throw new ArgumentNullException(nameof(days), nameof(_cfg));
    OCA = await _opnwea.GetIt(mgk, OpenWea.OpenWeatherCd.OneCallApi) as RootobjectOneCallApi; ArgumentNullException.ThrowIfNull(OCA); // PHC107
    D53 = await _opnwea.GetIt(mgk, OpenWea.OpenWeatherCd.Frc5Day3Hr) as RootobjectFrc5Day3Hr; ArgumentNullException.ThrowIfNull(D53); // PHC107

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
      //tmi: WriteLine($"/>D53:  {r.dt_txt}    {UnixToDt(r.dt)}    {UnixToDt(r.dt):MMM dd  HH:mm}     {r.weather[0].description,-26}       {r.main.temp_min,6:N1} ÷ {r.main.temp_max,4:N1}°    pop{r.pop * 100,3:N0}%      {r}");
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{r.weather[0].icon}@2x.png"));
      OpnWeaTip3[h3] = $"{OpenWea.UnixToDt(r.dt):MMM d  H:mm} \n\n    {r.weather[0].description}   \n    {r.main.temp:N1}°    pop {r.pop * 100:N0}%";
      h3++;
    });
#endif

    const int id = 2;
    await SetMaxX(id);
    TimeMin = DateTime.Today.AddDays(-1).ToOADate(); // == DateTimeAxis.ToDouble(DateTime.Today.AddDays(-1));
    TimeMax = DateTime.Today.AddDays(id).ToOADate(); // DateTimeAxis.ToDouble(_days == 5 ? UnixToDt(oca.daily.Max(d => d.dt) + 12 * 3600) : DateTime.Today.AddDays(_days));
    var valueMax = _extrMax; // oca.daily.Max(r => r.temp.max);
    var valueMin = _extrMin; // oca.daily.Min(r => r.temp.min);

    if (_cfg["StoreDataToDB"] == "Yes") //if (_cfg["StoreDataToDB"] == "Yes") //if (Environment.MachineName != "LR6WB43X")
      await AddForeDataToDB_OpnWea("phc", OCA);

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

    await Tick();

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

    await Tick();

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

    await Tick();

    OwaLoclGus_.Clear();

    var t0 = OpenWea.UnixToDt(x.sunrise).ToOADate();
    var dh = 16 * Math.Cos(t0 * Math.PI * 2);

    FunctionSeries__(Math.Cos, t0 - 1.3, t0 + 7.4, .0125);
    void FunctionSeries__(Func<double, double> f, double x0, double x1, double dx)
    {
      for (var t = x0; t <= x1 + (dx * 0.5); t += dx)
        OwaLoclGus_.Add(new DataPoint(t, dh - (16 * f(t * Math.PI * 2))));
    }

    await Tick();

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

    //oca.daily.ToList().ForEach(x =>
    //{
    //  SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), size: 3, y: x.temp.morn));
    //  SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), size: 5, y: x.temp.day));
    //  SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), size: 7, y: x.temp.eve));
    //  SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), size: 9, y: x.temp.night));
    //});
  }

  void Clear()
  {
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
  [ObservableProperty] string plotTitle = "°";
  [ObservableProperty] string currentConditions = "°";
  [ObservableProperty] string curTempReal = "°";
  [ObservableProperty] string curTempFeel = "°";
  [ObservableProperty] string curWindKmHr = "°";
  [ObservableProperty] int windDirn;
  [ObservableProperty] float windVeloKmHr;
  [ObservableProperty] RootobjectOneCallApi? oCA = default!;
  [ObservableProperty] RootobjectFrc5Day3Hr? d53 = default!;

  //ImageSource _i; public ImageSource WeaIcom { get => _i; set => SetProperty(ref _i, value); }
  //Uri _k = new("http://openweathermap.org/img/wn/04n@2x.png"); public Uri WIcon { get => _k; set => SetProperty(ref _k, value); }
  const float _wk = 10f, _kprsr = .01f;
  const float _kWind = 3.6f * _wk;

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
  [ObservableProperty] double yAxiXMax = -01;
  [ObservableProperty] double yAxiXMin = -1;
  [ObservableProperty] double windGustKmHr;
  [ObservableProperty] IInterpolationAlgorithm iA = InterpolationAlgorithms.CatmullRomSpline; // the least vertical jumping beyond y value.

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

    await Task.Yield();// PopulateAll((int?)_days ?? 5);
  }
}

/// <summary>
/// todo: assigns but not refreshes.
/// </summary>
public class Messages : ObservableObject // https://stackoverflow.com/questions/52921418/how-can-i-use-an-array-in-a-viewmodel
{
  readonly IDictionary<int, string> _messages = new Dictionary<int, string>();

  [IndexerName("Item")] //not exactly needed as this is the default
  public string this[int index]
  {
    get => _messages.TryGetValue(index, out var value) ? value : "http://openweathermap.org/img/wn/02d@2x.png";      /////////////////////////todo: never gets called.
    set { _messages[index] = value; OnPropertyChanged("Item[" + index + "]"); }
  }
}

///todo: https://oxyplot.readthedocs.io/en/latest/models/series/ScatterSeries.html
///https://docs.microsoft.com/en-us/answers/questions/22863/how-to-customize-charts-in-wpf-using-systemwindows.html
///https://docs.microsoft.com/en-us/answers/questions/10086/draw-chart-with-systemwindowscontrolsdatavisualiza.html
