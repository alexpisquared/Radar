internal static class PlotViewModelHelpers
{
  const string _toronto = "s0000458", _torIsld = "s0000785", _mississ = "s0000786", _vaughan = "s0000584", _markham = "s0000585", _richmhl = "s0000773", _newmark = "s0000582",
_phc = "phc", _vgn = "vgn", _mis = "mis",
_urlPast24hrYYZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=yyz", // Pearson
_urlPast24hrYKZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=ykz"; // Buttonville

  internal static async Task AddForeDataToDB_EnvtCa(WeatherxContext _dbx, string siteId, siteData? siteFore, string srcId = "eca", string measureId = "tar")
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
          d.MeasureValue == val && // store only changes in recalculation results
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
  internal static async Task AddForeDataToDB_OpnWea(WeatherxContext _dbx, string siteId, RootobjectOneCallApi? siteFore, string srcId = "owa", string measureId = "tar")
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
          d.MeasureValue == f.temp && // store only changes in recalculation results
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
  internal static async Task AddPastDataToDB_EnvtCa(WeatherxContext _dbx, string siteId, List<MeteoDataMy> sitePast, string srcId = "eca", string measureId = "tar")
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

  internal static DateTimeOffset EnvtCaDate(string yyyyMMddHHmm)
  {
    if (!DateTime.TryParseExact(yyyyMMddHHmm, new string[] { "yyyyMMddHHmm", "yyyyMMddHHmmss" }, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var utc)) //var lcl = DateTime.ParseExact(yyyyMMddHHmm, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToLocalTime(); //tu: UTC to Local time.
      throw new ArgumentOutOfRangeException(nameof(yyyyMMddHHmm), yyyyMMddHHmm, "@@@@@@@@ Can you imagine?!?!?!");

    return utc.ToLocalTime(); //tu: UTC to Local time.
  }

  internal static DateTimeOffset EnvtCaDate(dateStampType dateStampType)
  {
    if (!DateTimeOffset.TryParseExact($"{dateStampType.year}-{dateStampType.month.Value}-{dateStampType.day.Value} {dateStampType.hour.Value}:{dateStampType.minute}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
      throw new ArgumentException(nameof(dateStampNameType));

    return dt;
  }

  internal static void RefillForeEnvtCa(ObservableCollection<DataPoint> points, siteData? siteDt)
  {
    ArgumentNullException.ThrowIfNull(siteDt, $"@@@@@@@@@ {nameof(siteDt)}");

    siteDt.hourlyForecastGroup.hourlyForecast.ToList().ForEach(x => points.Add(new DataPoint(DateTimeAxis.ToDouble(PlotViewModelHelpers.EnvtCaDate(x.dateTimeUTC).DateTime), double.Parse(x.temperature.Value))));
  }

  internal static void RefillPast24(ObservableCollection<DataPoint> temps, ObservableCollection<DataPoint> winds, List<MeteoDataMy>? siteDt, float _wk)
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

  internal static async Task<(siteData? sitedataMiss, siteData? sitedataVghn)> GetFore24hrFromEC()
  {
    var sitedataMiss = await OpenWea.GetEnvtCa(_mississ);
    var sitedataVghn = await OpenWea.GetEnvtCa(_vaughan);

    return (sitedataMiss, sitedataVghn);
  }
  internal static async Task<(List<MeteoDataMy> bvl, List<MeteoDataMy> pea)> GetPast24hrFromEC()
  {
    Past24hrHAP p24 = new();
    var bvl = await p24.GetIt(_urlPast24hrYYZ);
    var pea = await p24.GetIt(_urlPast24hrYKZ);
    return (bvl, pea);
  }
}