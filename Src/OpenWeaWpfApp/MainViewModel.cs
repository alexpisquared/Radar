﻿using System.ComponentModel.Design.Serialization;
using DB.WeatherX.PwrTls;
using OpenWeather2022.Response;

namespace OpenWeaWpfApp;

public class MainViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableValidator
{
  readonly IConfigurationRoot _config;
  readonly OpenWea _opnwea;
  readonly int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600, _d3r = 4, _d3c = 600, _windClr = 333, _popClr = 0;
  readonly WeatherxContext _dbx;
  const string _toronto = "s0000458", _torIsld = "s0000785", _mississ = "s0000786", _vaughan = "s0000584", _markham = "s0000585", _richmhl = "s0000773", _newmark = "s0000582",
    _phc = "phc", _vgn = "vgn", _mis = "mis",
    _urlPast24hrYYZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=yyz", // Pearson
    _urlPast24hrYKZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=ykz"; // Buttonville

  public MainViewModel(WeatherxContext weatherxContext, OpenWea openWea)
  {
    _config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    _opnwea = openWea;
    _dbx = weatherxContext;
    WriteLine($"*** {_dbx.Database.GetConnectionString()}");
  }

  public async Task<bool> PopulateAsync()
  {
    try
    { //await Task.Delay(999); no diff
      Clear();

      await PrevForecastFromDB();
      await PopulateEnvtCanaAsync();
      await PopulateScatModelAsync();
      //await PopulateFuncModelAsync();
      //await PopulateOpenWeathAsync(); -- extra calls 

      Beep.Play();
    }
    catch (Exception ex)
    {
      WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@"); 
      if (Debugger.IsAttached) Debugger.Break(); else MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
      return false;
    }

    return true;
  }

  async Task PrevForecastFromDB()
  {
    var now = DateTime.Now;
    var ytd = DateTime.Now.AddHours(-24);
    var dby = DateTime.Now.AddHours(-48);

    (await _dbx.PointFore.Where(r => r.SiteId == _phc && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + .25 * (r.ForecastedFor - r.ForecastedAt).TotalHours, y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
    (await _dbx.PointFore.Where(r => r.SiteId == _vgn && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFVgn.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + .25 * (r.ForecastedFor - r.ForecastedAt).TotalHours, y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
    (await _dbx.PointFore.Where(r => r.SiteId == _mis && dby < r.ForecastedAt && ytd < r.ForecastedFor && r.ForecastedFor < now).ToListAsync()).ForEach(r => SctrPtTPFMis.Add(new ScatterPoint(DateTimeAxis.ToDouble(r.ForecastedFor.DateTime), size: 3 + .25 * (r.ForecastedFor - r.ForecastedAt).TotalHours, y: r.MeasureValue, tag: $"\r\npre:{(r.ForecastedFor - r.ForecastedAt).TotalHours:N1}h")));
  }

  async Task PopulateEnvtCanaAsync()
  {
    Past24hrHAP p24 = new();
    var b = p24.GetIt(_urlPast24hrYKZ);
    var p = p24.GetIt(_urlPast24hrYYZ);

    await AddPastDataToDB_EnvtCa("bvl", b);
    await AddPastDataToDB_EnvtCa("pea", p);

    RefillPast24(EnvtCaPast24ButtnvlT, EnvtCaPast24PearWind, b);
    RefillPast24(EnvtCaPast24PearsonT, EnvtCaPast24BtnvWind, p);

    var sitedataMiss = await _opnwea.GetEnvtCa(_mississ);
    var sitedataVghn = await _opnwea.GetEnvtCa(_vaughan);

    await AddForeDataToDB_EnvtCa("mis", sitedataMiss);
    await AddForeDataToDB_EnvtCa("vgn", sitedataVghn);

    RefillForeEnvtCa(EnvtCaToronto, await _opnwea.GetEnvtCa(_toronto));
    RefillForeEnvtCa(EnvtCaTorIsld, await _opnwea.GetEnvtCa(_torIsld));    //refill(EnvtCaTorIsld, await _opnwea.GetEnvtCa(_newmark));
    RefillForeEnvtCa(EnvtCaMissuga, sitedataMiss);
    RefillForeEnvtCa(EnvtCaVaughan, sitedataVghn);
    RefillForeEnvtCa(EnvtCaMarkham, await _opnwea.GetEnvtCa(_markham));
    RefillForeEnvtCa(EnvtCaRchmdHl, await _opnwea.GetEnvtCa(_richmhl));
    RefillForeEnvtCa(EnvtCaRchmdHl, await _opnwea.GetEnvtCa(_richmhl));

    EnvtCaIconM = ($"https://weather.gc.ca/weathericons/{(sitedataMiss?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"); // img1.Source = new BitmapImage(new Uri($"https://weather.gc.ca/weathericons/{(sitedata?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"));
    EnvtCaIconV = ($"https://weather.gc.ca/weathericons/{(sitedataVghn?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"); // img1.Source = new BitmapImage(new Uri($"https://weather.gc.ca/weathericons/{(sitedata?.currentConditions?.iconCode?.Value ?? "5"):0#}.gif"));
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
      throw new ArgumentException();
    return dt;
  }

  static void RefillPast24(ObservableCollection<DataPoint> temps, ObservableCollection<DataPoint> winds, List<MeteoDataMy>? siteDt)
  {
    ArgumentNullException.ThrowIfNull(siteDt, $"@@@@@@@@@ {nameof(siteDt)}");

    temps.Clear();
    winds.Clear();
    siteDt.ForEach(x =>
    {
      temps.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), x.TempAir));
      winds.Add(new DataPoint(DateTimeAxis.ToDouble(x.TakenAt), x.WindKmH * _wk));
    });
  }
  static void RefillForeEnvtCa(ObservableCollection<DataPoint> points, siteData? siteDt)
  {
    ArgumentNullException.ThrowIfNull(siteDt, $"@@@@@@@@@ {nameof(siteDt)}");

    points.Clear();
    siteDt.hourlyForecastGroup.hourlyForecast.ToList().ForEach(x => points.Add(new DataPoint(DateTimeAxis.ToDouble(EnvtCaDate(x.dateTimeUTC).DateTime), double.Parse(x.temperature.Value))));
  }

  async Task PopulateScatModelAsync()
  {
    DataPtNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), +10));
    DataPtNowT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), -25));

    OCA = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], OpenWeatherCd.OneCallApi) as RootobjectOneCallApi; ArgumentNullException.ThrowIfNull(OCA); // PHC107
    D53 = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], OpenWeatherCd.Frc5Day3Hr) as RootobjectFrc5Day3Hr; ArgumentNullException.ThrowIfNull(D53); // PHC107

    PlotTitle = $"{UnixToDt(OCA.current.dt):ddd HH:mm}    {OCA.current.temp:N1}°    {OCA.current.feels_like:N0}°    {OCA.current.wind_speed * _kWind:N1}k/h";
    CurrentConditions = $"{UnixToDt(OCA.current.dt):HH:mm:ss}\n  {OCA.current.temp,5:N1}°\n   {OCA.current.feels_like,4:N0}°\n  {OCA.current.wind_speed * _kWind:N1}k/h";
    WindDirn = OCA.current.wind_deg;
    WindVelo = OCA.current.wind_speed * _kWind * 10;
    OpnWeaIcom = $"http://openweathermap.org/img/wn/{OCA.current.weather.First().icon}@2x.png";

    var timeMin = DateTimeAxis.ToDouble(OpenWea.UnixToDt(OCA.daily.Min(d => d.dt - 07 * 3600)));
    var timeMax = DateTimeAxis.ToDouble(OpenWea.UnixToDt(OCA.daily.Max(d => d.dt + 12 * 3600)));
    var valueMin = OCA.daily.Min(r => r.temp.min);
    var valueMax = OCA.daily.Max(r => r.temp.max);

    await AddForeDataToDB_OpnWea("phc", OCA);

    OCA.hourly.ToList().ForEach(x =>
    {
      //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._1h ?? 0, x.snow?._1h ?? 0, _d3c)); // either null or 0 so far.
      DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp));
      DataPtFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.feels_like));
      DataPtWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed * _kWind));
      DataPtGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _kWind));
      DataPtPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
    });

    D53.list.Where(d => d.dt > OCA.hourly.Max(d => d.dt)).ToList().ForEach(x =>
    {
      //scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._3h ?? 0, x.snow?._3h ?? 0, _d3c));
      DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));
      DataPtFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like));
      DataPtWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _kWind));
      DataPtGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust * _kWind));
      DataPtPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
    });

    OCA.daily.ToList().ForEach(x =>
    {
      DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunrise)), valueMin));
      DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunrise)), valueMax));
      DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunset)), valueMax));
      DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunset)), valueMin));
    });

    OCA.daily
      .Where(d => d.dt > D53.list.Max(d => d.dt))
      .ToList().ForEach(x =>
      {
        DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.temp.morn));
        DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.temp.day));
        DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.temp.eve));
        DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.temp.night));
        DataPtWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed * _kWind));
        DataPtGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _kWind));
        DataPtPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
      });

    OCA.daily.ToList().ForEach(x =>
    {
      SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), value: 10, y: x.temp.morn));
      SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), value: 20, y: x.temp.day));
      SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), value: 30, y: x.temp.eve));
      SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), value: 40, y: x.temp.night));
    });
  }

  void Clear()
  {
    PlotTitle =
    CurrentConditions = $"Loading...";
    WindDirn = 0;
    WindVelo = 0;
    OpnWeaIcom = "http://openweathermap.org/img/wn/01n@2x.png";
    DataPtGust.Clear();
    DataPtWind.Clear();
    DataPtTemp.Clear();
    DataPtFeel.Clear();
    DataPtFeel.Clear();
    DataPtSunT.Clear();
    DataPtPopr.Clear();
  }

  void CopyOpenWeaToDataPtLists(RootobjectOneCallApi? ocv, ObservableCollection<DataPoint> pointsTemp, ObservableCollection<DataPoint> pointsFeel)
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

  public ObservableCollection<ScatterPoint> SctrPtTFFPhc { get; } = new ObservableCollection<ScatterPoint>();
  public ObservableCollection<ScatterPoint> SctrPtTPFVgn { get; } = new ObservableCollection<ScatterPoint>();
  public ObservableCollection<ScatterPoint> SctrPtTPFPhc { get; } = new ObservableCollection<ScatterPoint>();
  public ObservableCollection<ScatterPoint> SctrPtTPFMis { get; } = new ObservableCollection<ScatterPoint>();
  public ObservableCollection<DataPoint> DataPtTemp { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtFeel { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtWind { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtGust { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtSunT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtNowT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtPopr { get; } = new ObservableCollection<DataPoint>();

  public ObservableCollection<DataPoint> DataPtTempC { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtFeelC { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtTempT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> DataPtFeelT { get; } = new ObservableCollection<DataPoint>();

  public ObservableCollection<DataPoint> EnvtCaPast24PearsonT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> EnvtCaPast24ButtnvlT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> EnvtCaPast24PearWind { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> EnvtCaPast24BtnvWind { get; } = new ObservableCollection<DataPoint>();

  public ObservableCollection<DataPoint> EnvtCaToronto { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> EnvtCaVaughan { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> EnvtCaMarkham { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> EnvtCaMissuga { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> EnvtCaRchmdHl { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> EnvtCaTorIsld { get; } = new ObservableCollection<DataPoint>();

  public PlotModel FuncModel { get; private set; } = new PlotModel { Title = "Function Srs", Background = OxyColor.FromUInt32(123456), LegendTitleColor = OxyColor.FromUInt32(123456) };
  public PlotModel ScatModel { get; private set; } = new PlotModel { Title = "Scatter Srs" };

  string _t = default!; public string PlotTitle { get => _t; set => SetProperty(ref _t, value); }
  string _c = default!; public string CurrentConditions { get => _c; set => SetProperty(ref _c, value); }
  int _w = default!; public int WindDirn { get => _w; set => SetProperty(ref _w, value); }
  float _v = default!; public float WindVelo { get => _v; set => SetProperty(ref _v, value); }
  RootobjectOneCallApi? _o = default!; public RootobjectOneCallApi? OCA { get => _o; set => SetProperty(ref _o, value); }
  RootobjectFrc5Day3Hr? _f = default!; public RootobjectFrc5Day3Hr? D53 { get => _f; set => SetProperty(ref _f, value); }

  //ImageSource _i; public ImageSource WeaIcom { get => _i; set => SetProperty(ref _i, value); }
  //Uri _k = new("http://openweathermap.org/img/wn/04n@2x.png"); public Uri WIcon { get => _k; set => SetProperty(ref _k, value); }
  const float _wk = .1f;
  const float _kWind = 3.6f * _wk;

  string _j = "http://openweathermap.org/img/wn/01d@2x.png"; public string OpnWeaIcom { get => _j; set => SetProperty(ref _j, value); }
  string _i = "https://weather.gc.ca/weathericons/05.gif"; public string EnvtCaIconM { get => _i; set => SetProperty(ref _i, value); }
  string _k = "https://weather.gc.ca/weathericons/05.gif"; public string EnvtCaIconV { get => _k; set => SetProperty(ref _k, value); }
}
///todo: https://oxyplot.readthedocs.io/en/latest/models/series/ScatterSeries.html
///https://docs.microsoft.com/en-us/answers/questions/22863/how-to-customize-charts-in-wpf-using-systemwindows.html
///https://docs.microsoft.com/en-us/answers/questions/10086/draw-chart-with-systemwindowscontrolsdatavisualiza.html

/*
  async Task PopulateScatModelAsync_TogetherWithPlotView()
  {
    DataPtGust.Clear();
    DataPtWind.Clear();
    DataPtTemp.Clear();
    DataPtFeel.Clear();
    DataPtFeel.Clear();
    DataPtSunT.Clear();

    var oca = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], OpenWeatherCd.OneCallApi) as RootobjectOneCallApi; ArgumentNullException.ThrowIfNull(oca); // PHC107
    var d53 = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], OpenWeatherCd.Frc5Day3Hr) as RootobjectFrc5Day3Hr; ArgumentNullException.ThrowIfNull(d53); // PHC107

    var scaters = new ScatterSeries { MarkerType = MarkerType.Triangle, MarkerSize = 11 };
    var lsDaily = new LineSeries { Color = OxyColor.FromRgb(255, 00, 00), BrokenLineStyle = LineStyle.Solid, StrokeThickness = 1 };
    var lsD5H3y = new LineSeries { Color = OxyColor.FromRgb(255, 255, 0), BrokenLineStyle = LineStyle.Dot, StrokeThickness = 1 };

    var timeMin = DateTimeAxis.ToDouble(OpenWea.UnixToDt(oca.daily.Min(d => d.dt - 07 * 3600)));
    var timeMax = DateTimeAxis.ToDouble(OpenWea.UnixToDt(oca.daily.Max(d => d.dt + 12 * 3600)));
    var valueMin = oca.daily.Min(r => r.temp.min);
    var valueMax = oca.daily.Max(r => r.temp.max);

    var gridColor = OxyColor.FromRgb(22, 22, 22);
    ScatModel.Title = $"{OpenWea.UnixToDt(oca.current.dt):ddd HH:mm}  {oca.current.temp:N1}°  {oca.current.feels_like:N0}°  {oca.current.wind_deg}";
    ScatModel.Axes.Add(new LinearAxis
    {
      Position = AxisPosition.Left,
      TextColor = OxyColors.GreenYellow,
      MajorGridlineColor = gridColor,
      Key = "yAxis",
      IsZoomEnabled = false,
      IsPanEnabled = false,
      MajorGridlineStyle = LineStyle.Solid,
      Title = "Temp [°C]"
    });
    ScatModel.Axes.Add(new DateTimeAxis
    {
      Position = AxisPosition.Bottom,
      Minimum = timeMin,
      Maximum = timeMax,
      MajorGridlineColor = gridColor,
      IsZoomEnabled = false,
      IsPanEnabled = false,
      MajorGridlineStyle = LineStyle.Solid,
      StringFormat = "ddd H",
      TextColor = OxyColors.Red
    });
    ScatModel.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Rainbow(1000), TextColor = OxyColors.WhiteSmoke });

    oca.hourly.ToList().ForEach(x =>
    {
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._1h ?? 0, x.snow?._1h ?? 0, _d3c)); // either null or 0 so far.
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10, x.pop * 10, _popClr));

      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp, _d3r, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.feels_like, 2, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed * _kWind, _d3r, _windClr));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _kWind, 3, _windClr));

      //if (x.snow?._1h == 0) WriteLine(x.snow); else WriteLine(x.snow);

      DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp));
      DataPtFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.feels_like));
      DataPtWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed * _kWind));
      DataPtGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _kWind));
      DataPtPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
    });

    d53.list.Where(d => d.dt > oca.hourly.Max(d => d.dt)).ToList().ForEach(x =>
    {
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._3h ?? 0, x.snow?._3h ?? 0, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10, x.pop * 10, _popClr));

      lsD5H3y.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));

      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp, _d3r, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like, 2, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _kWind, _d3r, _windClr));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust * _kWind, 2, _windClr));

      DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));
      DataPtFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like));
      DataPtWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed * _kWind));
      DataPtGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust * _kWind));
      DataPtPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
    });

    DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueMax - 5));
    DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueMin));
    oca.daily.ToList().ForEach(x =>
    {
      if (OpenWea.UnixToDt(x.sunset) > DateTime.Now)
      {
        DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunrise)), valueMin));
        DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunrise)), valueMax));
        DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunset)), valueMax));
        DataPtSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunset)), valueMin));
      }
    });

    oca.daily
      .Where(d => d.dt > d53.list.Max(d => d.dt))
      .ToList().ForEach(x =>
    {
      DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.temp.morn));
      DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.temp.day));
      DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.temp.eve));
      DataPtTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.temp.night));
      DataPtWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed * _kWind));
      DataPtGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _kWind));
      DataPtPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
    });

    oca.daily.ToList().ForEach(x =>
    {
      SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), value: 20, tag: $"{x.temp.morn} ", y: x.temp.morn));
      SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), value: 20, tag: $"{x.temp.day}  ", y: x.temp.day));
      SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), value: 20, tag: $"{x.temp.eve}  ", y: x.temp.eve));
      SctrPtTFFPhc.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), value: 20, tag: $"{x.temp.night}", y: x.temp.night));
    });

    oca.daily.
      //Where(d => d.dt > d53.list.Min(d => d.dt)).
      ToList().ForEach(x =>
      {
        scaters.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp.morn, 22, 1000) { });

        scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.temp.morn, 12, 0));
        scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.temp.day, 12, 0));
        scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.temp.eve, 12, 0));
        scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.temp.night, 12, 0));
        //lsDaily.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.temp.morn));
        //lsDaily.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.temp.day));
        //lsDaily.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.temp.eve));
        //lsDaily.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.temp.night));

        scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.feels_like.morn, 2, 0));
        scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.feels_like.day, 2, 750));
        scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.feels_like.eve, 2, 1000));
        scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.feels_like.night, 2, 333));

        scaters.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed * _kWind, 3, _windClr));
        scaters.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust * _kWind, 2, _windClr));
        scaters.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10, x.pop, 700));
        scaters.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow, x.snow, 990));
        scaters.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.rain, x.rain, 500));
      });

    ScatModel.Series.Add(scaters);
    ScatModel.Series.Add(lsDaily);
    ScatModel.Series.Add(lsD5H3y);
  }
  async Task PopulateOpenWeathAsync()
  {
    var occ = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], OpenWeatherCd.OneCallApi, 43.8374229, -79.4961442) as RootobjectOneCallApi; // PHC107
    var oct = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], OpenWeatherCd.OneCallApi, 43.7181557, -79.5181414) as RootobjectOneCallApi; // 400x401

    CopyOpenWeaToDataPtLists(occ, DataPtTempC, DataPtFeelC);
    CopyOpenWeaToDataPtLists(oct, DataPtTempT, DataPtFeelT);
  }
  async Task PopulateFuncModelAsync() { await Task.Yield(); FuncModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)")); }

 */