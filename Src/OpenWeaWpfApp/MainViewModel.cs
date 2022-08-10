#define ObsCol // Go figure: ObsCol works, while array NOT! Just an interesting factoid.
namespace OpenWeaWpfApp;
public class MainViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableValidator
{
  readonly IConfigurationRoot _config;
  readonly OpenWea _opnwea;
  readonly int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600, _yHi = 2, _yLo = 23;
  const int _maxIcons = 50;
  readonly WeatherxContext _dbx;
  double _extrMax = +20, _extrMin = -20;
  const string _toronto = "s0000458", _torIsld = "s0000785", _mississ = "s0000786", _vaughan = "s0000584", _markham = "s0000585", _richmhl = "s0000773", _newmark = "s0000582",
    _phc = "phc", _vgn = "vgn", _mis = "mis",
    _urlPast24hrYYZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=yyz", // Pearson
    _urlPast24hrYKZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=ykz"; // Buttonville

  public MainViewModel(WeatherxContext weatherxContext, OpenWea openWea)
  {
    _config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    _opnwea = openWea;
    _dbx = weatherxContext; // WriteLine($"*** {_dbx.Database.GetConnectionString()}"); // 480ms

    for (var i = 0; i < _maxIcons; i++)
    {
#if ObsCol // must add for the binding to have something to hook on to for OpnWea[xx].
      OpnWeaIco3.Add(new BitmapImage(new Uri("http://openweathermap.org/img/wn/01d.png")));
      OpnWeaTip3.Add("Optimistic Init");
#else
      OpnWeaIco3[i]=(new BitmapImage(new Uri("http://openweathermap.org/img/wn/01d.png")));
      OpnWeaTip3[i]=("Optimistic Init");
#endif
    }
  }

  public async Task<bool> PopulateAsync(int days = 5)
  {
    _busy = true;
    try
    {
      Clear();

      await PrevForecastFromDB();
      await PopulateEnvtCanaAsync();
      await PopulateScatModelAsync(days);

      Beep.Play();
    }
    catch (Exception ex)
    {
      WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@");
      if (Debugger.IsAttached) Debugger.Break(); else MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
      return false;
    }
    finally
    {
      _busy = false;
    }

    return true;
  }

  async Task PrevForecastFromDB()
  {
    if (_config["StoreData"] != "Yes") //if (Environment.MachineName != "D21-MJ0AWBEV")
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
        WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@");
        if (Debugger.IsAttached) Debugger.Break(); else MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
      }
    }

    (await _dbx.PointFore.Where(r => r.SiteId == _phc && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + .25 * (r.ForecastedFor - r.ForecastedAt).TotalHours, y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
    (await _dbx.PointFore.Where(r => r.SiteId == _vgn && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFVgn.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + .25 * (r.ForecastedFor - r.ForecastedAt).TotalHours, y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
    (await _dbx.PointFore.Where(r => r.SiteId == _mis && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFMis.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + .25 * (r.ForecastedFor - r.ForecastedAt).TotalHours, y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
  }
  async Task PopulateEnvtCanaAsync()
  {
    Past24hrHAP p24 = new();
    var bvl = p24.GetIt(_urlPast24hrYKZ);
    var pea = p24.GetIt(_urlPast24hrYYZ);

    if (_config["StoreData"] == "Yes") //if (Environment.MachineName != "D21-MJ0AWBEV")         "StoreData": "No",
    {
      await AddPastDataToDB_EnvtCa("bvl", bvl);
      await AddPastDataToDB_EnvtCa("pea", pea);
    }

    RefillPast24(ECaVghnTemp, ECaPearWind, bvl);
    RefillPast24(ECaMissTemp, ECaBtvlWind, pea);

    OwaLoclPrsr.Clear(); pea.OrderBy(r => r.TakenAt).ToList().ForEach(x => OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), 10 * x.Pressure - 1030)));

    var sitedataMiss = await _opnwea.GetEnvtCa(_mississ);
    var sitedataVghn = await _opnwea.GetEnvtCa(_vaughan);

    if (_config["StoreData"] == "Yes") //if (Environment.MachineName != "D21-MJ0AWBEV")
    {
      await AddForeDataToDB_EnvtCa("mis", sitedataMiss);
      await AddForeDataToDB_EnvtCa("vgn", sitedataVghn);
    }

    RefillForeEnvtCa(ECaToroTemp, await _opnwea.GetEnvtCa(_toronto));
    RefillForeEnvtCa(ECaTIslTemp, await _opnwea.GetEnvtCa(_torIsld));    //refill(ECaTIslTemp, await _opnwea.GetEnvtCa(_newmark));
    RefillForeEnvtCa(ECaMissTemp, sitedataMiss);
    RefillForeEnvtCa(ECaVghnTemp, sitedataVghn);
    RefillForeEnvtCa(ECaMrkhTemp, await _opnwea.GetEnvtCa(_markham));

    ArgumentNullException.ThrowIfNull(sitedataMiss, $"@@@@@@@@@ {nameof(sitedataMiss)}");
    SubHeader += $"{sitedataMiss.currentConditions.wind.speed}\n";

    if (double.TryParse(sitedataMiss.almanac.temperature[0].Value, out var d)) { YAxiXMax = 200 + 10 * (YAxisMax = d + _yHi); _extrMax = d; }
    if (double.TryParse(sitedataMiss.almanac.temperature[1].Value, out /**/d)) { YAxiXMin = 200 + 10 * (YAxisMin = Math.Floor(d / 10) * 10 - _yLo); _extrMin = d; }
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

    //var connectionString = _config.GetConnectionString("Exprs");
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
        _dbx.PointReal.Add(new PointReal
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

    //var connectionString = _config.GetConnectionString("Exprs");
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
          d.MeasureValue == val && // store only changes in recalculation results
          d.ForecastedFor == forecastedFor) == false)
      {
        _dbx.PointFore.Add(new PointFore
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

    //var connectionString = _config.GetConnectionString("Exprs");
    //WeatherxContextFactory dbf = new(connectionString);
    //using WeatherxContext _dbx = dbf.CreateDbContext();

    var forecastedAt = OpenWea.UnixToDt(siteFore.current.dt);

    foreach (var f in siteFore.hourly.ToList()) //siteFore.hourlyForecastGroup.hourlyForecast.ToList().ForEach(async f =>
    {
      var forecastedFor = OpenWea.UnixToDt(f.dt);

      if (await _dbx.PointFore.AnyAsync(d =>
          d.SrcId == srcId &&
          d.SiteId == siteId &&
          d.MeasureValue == f.temp && // store only changes in recalculation results
          d.MeasureId == measureId &&
          d.ForecastedFor == forecastedFor) == false)
      {
        _dbx.PointFore.Add(new PointFore
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
      throw new ArgumentOutOfRangeException(nameof(yyyyMMddHHmm), yyyyMMddHHmm, "@@@@@@@@ Can you imagine?!?!?!");

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

    //points.Clear();
    siteDt.hourlyForecastGroup.hourlyForecast.ToList().ForEach(x => points.Add(new DataPoint(DateTimeAxis.ToDouble(EnvtCaDate(x.dateTimeUTC).DateTime), double.Parse(x.temperature.Value))));
  }

  async Task PopulateScatModelAsync(int days = 5)
  {
    OCA = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], OpenWeatherCd.OneCallApi) as RootobjectOneCallApi; ArgumentNullException.ThrowIfNull(OCA); // PHC107
    D53 = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], OpenWeatherCd.Frc5Day3Hr) as RootobjectFrc5Day3Hr; ArgumentNullException.ThrowIfNull(D53); // PHC107

    SubHeader += $"{OCA.current}";
    PlotTitle = CurrentConditions = $"{UnixToDt(OCA.current.dt):HH:mm:ss}   {OCA.current.temp,5:N1}°   {OCA.current.feels_like,4:N0}°  {OCA.current.wind_speed * _kWind:N1}k/h";
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
    for (; h3 < UnixToDt(D53.list[0].dt).Hour / 3; h3++)
    {
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/01n.png", UriKind.Absolute)); //nogo: OpnWeaIco3[h3] = new BitmapImage(new Uri($"/Views/NoDataIndicator.bmp", UriKind.Absolute));
      OpnWeaTip3[h3] = ($"{h3}");
    }
    D53.list.ToList().ForEach(r =>
    {
      WriteLine($"D53:  {r.dt_txt}    {UnixToDt(r.dt)}    {UnixToDt(r.dt):MMM dd  HH:mm}     {r.weather[0].description,-26}       {r.main.temp_min,6:N1} ÷ {r.main.temp_max,4:N1}°    pop{r.pop * 100,3:N0}%      {r}");
      OpnWeaIco3[h3] = new BitmapImage(new Uri($"http://openweathermap.org/img/wn/{r.weather[0].icon}@2x.png"));
      OpnWeaTip3[h3] = ($"{UnixToDt(r.dt):MMM d  H:mm} \n\n    {r.weather[0].description}   \n    {r.main.temp:N1}°    pop {r.pop * 100:N0}%");
      h3++;
    });
#endif

    const int id = 2;
    await SetMaxX(id);
    TimeMin = DateTime.Today.AddDays(-1).ToOADate(); // == DateTimeAxis.ToDouble(DateTime.Today.AddDays(-1));
    TimeMax = DateTime.Today.AddDays(id).ToOADate(); // DateTimeAxis.ToDouble(days == 5 ? UnixToDt(OCA.daily.Max(d => d.dt) + 12 * 3600) : DateTime.Today.AddDays(days));
    var valueMax = _extrMax; // OCA.daily.Max(r => r.temp.max);
    var valueMin = _extrMin; // OCA.daily.Min(r => r.temp.min);

    if (_config["StoreData"] == "Yes") //if (_config["StoreData"] == "Yes") //if (Environment.MachineName != "D21-MJ0AWBEV")
      await AddForeDataToDB_OpnWea("phc", OCA);

    OCA.hourly.ToList().ForEach(x =>
    {
      //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._1h ?? 0, x.snow?._1h ?? 0, _d3c)); // either null or 0 so far.
      OwaLoclTemp.Add(new DataPoint(DateTimeAxis.ToDouble(UnixToDt(x.dt)), x.temp));
      OwaLoclFeel.Add(new DataPoint(DateTimeAxis.ToDouble(UnixToDt(x.dt)), x.feels_like));
      OwaLoclPrsr.Add(new DataPoint(DateTimeAxis.ToDouble(UnixToDt(x.dt)), x.pressure - 1030));
      OwaLoclGust.Add(new DataPoint(DateTimeAxis.ToDouble(UnixToDt(x.dt)), x.wind_gust * _kWind));
      OwaLoclPopr.Add(new DataPoint(DateTimeAxis.ToDouble(UnixToDt(x.dt)), x.pop * 100));

      var rad = Math.PI * x.wind_deg * 2 / 360;
      var dx = 0.10 * Math.Cos(rad);
      var dy = 10.0 * Math.Sin(rad);
      var tx = .002 * Math.Cos(rad + 90);
      var ty = 0.20 * Math.Sin(rad + 90);
      var sx = .002 * Math.Cos(rad - 90);
      var sy = 0.20 * Math.Sin(rad - 90);
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(UnixToDt(x.dt)) + tx, ty + x.wind_speed * _kWind));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(UnixToDt(x.dt)) + dx, dy + x.wind_speed * _kWind));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(UnixToDt(x.dt)) + sx, sy + x.wind_speed * _kWind));
    });

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
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)) + dx, dy + x.wind.speed * _kWind));
      ECaBtvlWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _kWind));
    });

    var x = OCA.daily.First();
    OwaLoclSunT.Add(new DataPoint((UnixToDt(x.sunrise).AddDays(-1).ToOADate()), valueMin));
    OwaLoclSunT.Add(new DataPoint((UnixToDt(x.sunrise).AddDays(-1).ToOADate()), valueMax));
    OwaLoclSunT.Add(new DataPoint((UnixToDt(x.sunset).AddDays(-1).ToOADate()), valueMax));
    OwaLoclSunT.Add(new DataPoint((UnixToDt(x.sunset).AddDays(-1).ToOADate()), valueMin));
    OCA.daily.ToList().ForEach(x =>
    {
      OwaLoclSunT.Add(new DataPoint((UnixToDt(x.sunrise).ToOADate()), valueMin));
      OwaLoclSunT.Add(new DataPoint((UnixToDt(x.sunrise).ToOADate()), valueMax));
      OwaLoclSunT.Add(new DataPoint((UnixToDt(x.sunset).ToOADate()), valueMax));
      OwaLoclSunT.Add(new DataPoint((UnixToDt(x.sunset).ToOADate()), valueMin));
    });

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

  public ObservableCollection<ScatterPoint> SctrPtTPFVgn { get; } = new ObservableCollection<ScatterPoint>();
  public ObservableCollection<ScatterPoint> SctrPtTPFPhc { get; } = new ObservableCollection<ScatterPoint>();
  public ObservableCollection<ScatterPoint> SctrPtTPFMis { get; } = new ObservableCollection<ScatterPoint>();
  public ObservableCollection<DataPoint> OwaLoclTemp { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> OwaLoclFeel { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> OwaLoclPrsr { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> OwaLoclGust { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> ECaBtvlWind { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> ECaPearWind { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> OwaLoclSunT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> OwaLoclNowT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> OwaLoclPopr { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> ECaToroTemp { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> ECaVghnTemp { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> ECaMrkhTemp { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> ECaMissTemp { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> ECaTIslTemp { get; } = new ObservableCollection<DataPoint>();

  public PlotModel FuncModel { get; private set; } = new PlotModel { Title = "Function Srs", Background = OxyColor.FromUInt32(123456)/*, LegendTitleColor = OxyColor.FromUInt32(123456)*/ };
  public PlotModel ScatModel { get; private set; } = new PlotModel { Title = "Scatter Srs" };

  double _fn; public double TimeMin { get => _fn; set => SetProperty(ref _fn, value); }
  double _fm; public double TimeMax { get => _fm; set => SetProperty(ref _fm, value); }
  string _t = default!; public string PlotTitle { get => _t; set => SetProperty(ref _t, value); }
  string _c = default!; public string CurrentConditions { get => _c; set => SetProperty(ref _c, value); }
  string _r = default!; public string CurTempReal { get => _r; set => SetProperty(ref _r, value); }
  string _f = default!; public string CurTempFeel { get => _f; set => SetProperty(ref _f, value); }
  string _w = default!; public string CurWindKmHr { get => _w; set => SetProperty(ref _w, value); }
  int _b = default!; public int WindDirn { get => _b; set => SetProperty(ref _b, value); }
  float _v = default!; public float WindVeloKmHr { get => _v; set => SetProperty(ref _v, value); }
  RootobjectOneCallApi? _o = default!; public RootobjectOneCallApi? OCA { get => _o; set => SetProperty(ref _o, value); }
  RootobjectFrc5Day3Hr? _y = default!; public RootobjectFrc5Day3Hr? D53 { get => _y; set => SetProperty(ref _y, value); }

  //ImageSource _i; public ImageSource WeaIcom { get => _i; set => SetProperty(ref _i, value); }
  //Uri _k = new("http://openweathermap.org/img/wn/04n@2x.png"); public Uri WIcon { get => _k; set => SetProperty(ref _k, value); }
  const float _wk = 10f, _kprsr = .01f;
  const float _kWind = 3.6f * _wk;

  ObservableCollection<string> _a = new(); public ObservableCollection<string> OpnWeaIcoA { get => _a; set => SetProperty(ref _a, value); }
#if ObsCol
  ObservableCollection<ImageSource> _3 = new(); public ObservableCollection<ImageSource> OpnWeaIco3 { get => _3; set => SetProperty(ref _3, value); }
  ObservableCollection<string> _4 = new(); public ObservableCollection<string> OpnWeaTip3 { get => _4; set => SetProperty(ref _4, value); }
#else
  ImageSource[] _3 = new ImageSource[_maxIcons]; public ImageSource[] OpnWeaIco3 { get => _3; set => SetProperty(ref _3, value); }
  string[] _4 = new string[_maxIcons]; public string[] OpnWeaTip3 { get => _4; set => SetProperty(ref _4, value); }
  //Messages _m3 = new(); public Messages OpnWeaIco3 { get => _m3; set => SetProperty(ref _m3, value); }
  //Messages _m4 = new(); public Messages OpnWeaTip3 { get => _m4; set => SetProperty(ref _m4, value); }
#endif

  GridLength _iw0; public GridLength IconWidth0 { get => _iw0; set => SetProperty(ref _iw0, value); }
  GridLength _iw1; public GridLength IconWidth1 { get => _iw1; set => SetProperty(ref _iw1, value); }
  GridLength _iw2; public GridLength IconWidth2 { get => _iw2; set => SetProperty(ref _iw2, value); }
  GridLength _iw3; public GridLength IconWidth3 { get => _iw3; set => SetProperty(ref _iw3, value); }
  GridLength _iw4; public GridLength IconWidth4 { get => _iw4; set => SetProperty(ref _iw4, value); }
  GridLength _iw5; public GridLength IconWidth5 { get => _iw5; set => SetProperty(ref _iw5, value); }
  GridLength _iw6; public GridLength IconWidth6 { get => _iw6; set => SetProperty(ref _iw6, value); }
  GridLength _iw7; public GridLength IconWidth7 { get => _iw7; set => SetProperty(ref _iw7, value); }

  GridLength _00; public GridLength IconWidt00 { get => _00; set => SetProperty(ref _00, value); }
  GridLength _01; public GridLength IconWidt01 { get => _01; set => SetProperty(ref _01, value); }
  GridLength _02; public GridLength IconWidt02 { get => _02; set => SetProperty(ref _02, value); }
  GridLength _03; public GridLength IconWidt03 { get => _03; set => SetProperty(ref _03, value); }
  GridLength _04; public GridLength IconWidt04 { get => _04; set => SetProperty(ref _04, value); }
  GridLength _05; public GridLength IconWidt05 { get => _05; set => SetProperty(ref _05, value); }
  GridLength _06; public GridLength IconWidt06 { get => _06; set => SetProperty(ref _06, value); }
  GridLength _07; public GridLength IconWidt07 { get => _07; set => SetProperty(ref _07, value); }
  GridLength _08; public GridLength IconWidt08 { get => _08; set => SetProperty(ref _08, value); }
  GridLength _09; public GridLength IconWidt09 { get => _09; set => SetProperty(ref _09, value); }
  GridLength _10; public GridLength IconWidt10 { get => _10; set => SetProperty(ref _10, value); }
  GridLength _11; public GridLength IconWidt11 { get => _11; set => SetProperty(ref _11, value); }
  GridLength _12; public GridLength IconWidt12 { get => _12; set => SetProperty(ref _12, value); }
  GridLength _13; public GridLength IconWidt13 { get => _13; set => SetProperty(ref _13, value); }
  GridLength _14; public GridLength IconWidt14 { get => _14; set => SetProperty(ref _14, value); }
  GridLength _15; public GridLength IconWidt15 { get => _15; set => SetProperty(ref _15, value); }
  GridLength _16; public GridLength IconWidt16 { get => _16; set => SetProperty(ref _16, value); }
  GridLength _17; public GridLength IconWidt17 { get => _17; set => SetProperty(ref _17, value); }
  GridLength _18; public GridLength IconWidt18 { get => _18; set => SetProperty(ref _18, value); }
  GridLength _19; public GridLength IconWidt19 { get => _19; set => SetProperty(ref _19, value); }
  GridLength _20; public GridLength IconWidt20 { get => _20; set => SetProperty(ref _20, value); }
  GridLength _21; public GridLength IconWidt21 { get => _21; set => SetProperty(ref _21, value); }
  GridLength _22; public GridLength IconWidt22 { get => _22; set => SetProperty(ref _22, value); }
  GridLength _23; public GridLength IconWidt23 { get => _23; set => SetProperty(ref _23, value); }
  GridLength _24; public GridLength IconWidt24 { get => _24; set => SetProperty(ref _24, value); }
  GridLength _25; public GridLength IconWidt25 { get => _25; set => SetProperty(ref _25, value); }
  GridLength _26; public GridLength IconWidt26 { get => _26; set => SetProperty(ref _26, value); }
  GridLength _27; public GridLength IconWidt27 { get => _27; set => SetProperty(ref _27, value); }
  GridLength _28; public GridLength IconWidt28 { get => _28; set => SetProperty(ref _28, value); }
  GridLength _29; public GridLength IconWidt29 { get => _29; set => SetProperty(ref _29, value); }
  GridLength _30; public GridLength IconWidt30 { get => _30; set => SetProperty(ref _30, value); }
  GridLength _31; public GridLength IconWidt31 { get => _31; set => SetProperty(ref _31, value); }
  GridLength _32; public GridLength IconWidt32 { get => _32; set => SetProperty(ref _32, value); }
  GridLength _33; public GridLength IconWidt33 { get => _33; set => SetProperty(ref _33, value); }
  GridLength _34; public GridLength IconWidt34 { get => _34; set => SetProperty(ref _34, value); }
  GridLength _35; public GridLength IconWidt35 { get => _35; set => SetProperty(ref _35, value); }
  GridLength _36; public GridLength IconWidt36 { get => _36; set => SetProperty(ref _36, value); }
  GridLength _37; public GridLength IconWidt37 { get => _37; set => SetProperty(ref _37, value); }
  GridLength _38; public GridLength IconWidt38 { get => _38; set => SetProperty(ref _38, value); }
  GridLength _39; public GridLength IconWidt39 { get => _39; set => SetProperty(ref _39, value); }

  string _j = "http://openweathermap.org/img/wn/01d@2x.png"; public string OpnWeaIcom { get => _j; set => SetProperty(ref _j, value); }
  string _i = "https://weather.gc.ca/weathericons/05.gif"; public string EnvtCaIconM { get => _i; set => SetProperty(ref _i, value); }
  string _k = "https://weather.gc.ca/weathericons/05.gif"; public string EnvtCaIconV { get => _k; set => SetProperty(ref _k, value); }

  string _s = "Loading..."; public string SubHeader { get => _s; set => SetProperty(ref _s, value); }

  bool _busy;
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

    await Task.Yield();// PopulateAsync((int?)days ?? 5);
  }

  double _yMin = -28; public double YAxisMin { get => _yMin; set => SetProperty(ref _yMin, value); }
  double _yMax = +12; public double YAxisMax { get => _yMax; set => SetProperty(ref _yMax, value); }
  double _tMin = -08; public double NormTMin { get => _tMin; set => SetProperty(ref _tMin, value); }
  double _tMax = +02; public double NormTMax { get => _tMax; set => SetProperty(ref _tMax, value); }
  double _YMin = -01; public double YAxiXMin { get => _YMin; set => SetProperty(ref _YMin, value); }
  double _YMax = -01; public double YAxiXMax { get => _YMax; set => SetProperty(ref _YMax, value); }

  IInterpolationAlgorithm iA = InterpolationAlgorithms.CatmullRomSpline; public IInterpolationAlgorithm IA { get => iA; set => SetProperty(ref iA, value); } // the least vertical jumping beyond y value.

  double _wg; public double WindGustKmHr { get => _wg; set => SetProperty(ref _wg, value); }
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
    get => _messages.ContainsKey(index) ?
        _messages[index] : /////////////////////////todo: never gets called.
        "http://openweathermap.org/img/wn/02d@2x.png";

    set
    {
      _messages[index] = value;
      OnPropertyChanged("Item[" + index + "]");
    }
  }
}

///todo: https://oxyplot.readthedocs.io/en/latest/models/series/ScatterSeries.html
///https://docs.microsoft.com/en-us/answers/questions/22863/how-to-customize-charts-in-wpf-using-systemwindows.html
///https://docs.microsoft.com/en-us/answers/questions/10086/draw-chart-with-systemwindowscontrolsdatavisualiza.html
