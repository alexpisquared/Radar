﻿using OpenMeteoClient.Domain.Models;

internal static class Cnst
{
  public const string _kingcty = "s0000582", _toronto = "s0000458", _torIsld = "s0000785", _mississ = "s0000786", _vaugOLD = "s0000584", _markham = "s0000585", _richmhl = "s0000773", _newmark = "s0000582", _phc = "phc", _vgn = "vgn", _mis = "mis", _ome = "ome",
    pearson = "pea",
    kingCty = "kng",
  _Past24YYZ = @"http://weather.gc.ca/past_conditions/index_e.html?station=yyz", // Pearson   20.5 km from PHC  https://www.google.ca/maps/@43.6800000,-79.6300000,34m/data=!3m1!1e3?entry=ttu                                                                    
  _Past24OKN = @"http://weather.gc.ca/past_conditions/index_e.html?station=okn"; // King City 15.9 km from PHC  https://www.google.ca/maps/@43.9640351,-79.5739119,34m/data=!3m1!1e3?entry=ttu  :ykz.Buttonville station is gone Oct, 2023.
}
internal static class PlotViewModelHelpers
{
  static readonly Bpr bpr = new();

  internal static async Task<DateTimeOffset> LastTimeStoredToDb(ILogger _lgr, WeatherxContext weatherxContext)
  {
    try
    {
      return await weatherxContext.PointReal.MaxAsync(r => r.CreatedAt);
    }
    catch (Exception ex)
    {
      ex.Pop(_lgr);
      return DateTimeOffset.Now;
    }
  }

  internal static async Task AddForecastToDB_EnvtCa(ILogger _lgr, WeatherxContext _dbx, string siteId, siteData? siteFore, string srcId = "eca", string measureId = "tar")
  {
    for (var i = 0; i < 10; i++)
    {
      try
      {
        ArgumentNullException.ThrowIfNull(siteFore, $"e846 {nameof(siteFore)}");
        var now = DateTime.Now;

        //var connectionString = _cfg.GetConnectionString("Exprs");
        //WeatherxContextFactory dbf = new(connectionString);
        //using WeatherxContext _dbx = dbf.CreateDbContext();

        var forecastedAt = GetDateSafe(_lgr, siteFore);

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

#if DEBUG
        WriteLine($"■■  await _dbx.SaveChangesAsync()  suspended in DEBUG ■■");
#else
        WriteLine($"■■ {await _dbx.SaveChangesAsync()} rows saved ■■");
#endif
        return;
      }
      catch (InvalidOperationException ex) { _lgr.LogWarning($"F{i,3} {ex.Message}"); await Task.Delay(1000); bpr.Warn(); }
      catch (Exception ex) { ex.Pop(_lgr); }
    }
  }

  static DateTimeOffset GetDateSafe(ILogger _lgr, siteData siteFore)
  {
    try { for (var i = 0; i < siteFore.dateTime.Length; i++) WriteLine($"■■: [{i}] {EnvtCaDate(siteFore.dateTime[i]):yyyy-MM-dd HH:mm:ss.fff}  <==  {siteFore.dateTime[1]} "); } catch (Exception ex) { _lgr.LogWarning($"■■ {ex.Message} ■■"); }

    return EnvtCaDate(siteFore.dateTime[^1]); // looks like the [1] is the local time, and [0] is the UTC time.
  }

  [Obsolete]
  internal static async Task AddForecastToDB_OpnWea(ILogger _lgr, WeatherxContext _dbx, string siteId, RootobjectOneCallApi? siteData, string srcId = "owa", string measureId = "tar")
  {
    for (var i = 0; i < 10; i++)
    {
      try
      {
        ArgumentNullException.ThrowIfNull(siteData, $"@@@@@@@@@ {nameof(siteData)}");
        var now = DateTime.Now;

        //var connectionString = _cfg.GetConnectionString("Exprs");
        //WeatherxContextFactory dbf = new(connectionString);
        //using WeatherxContext _dbx = dbf.CreateDbContext();

        var forecastedAt = OpenWea.UnixToDt(siteData.current.dt); //nogo: any more: based on siteData.current which is NUL since gone NONEFREE!!!

        foreach (var f in siteData.hourly.ToList()) //siteFore.hourlyForecastGroup.hourlyForecast.ToList().ForEach(async f =>
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

        WriteLine($"■■ {await _dbx.SaveChangesAsync()} rows saved ■■");
        return;
      }
      catch (InvalidOperationException ex) { WriteLine($"WARN: O{i,3} {ex.Message}"); await Task.Delay(1000); bpr.Warn(); }
      catch (Exception ex) { ex.Pop(_lgr); }
    }
  }
  internal static async Task AddForecastToDB_OpnMto(ILogger _lgr, WeatherxContext _dbx, string siteId, WeatherForecast? siteData, string srcId = "omt", string measureId = "tar")
  {
    for (var i = 0; i < 10; i++)
    {
      try
      {
        ArgumentNullException.ThrowIfNull(siteData, $"@ Open Meteo Data{nameof(siteData)}");
        var now = DateTime.Now;

        var forecastedAt = (siteData.Current?.Time) ?? DateTime.Now;

        for (int h = 0; h < siteData.Hourly?.Time.Count; h++)
        {
          var forecastedFor = siteData.Hourly.Time[h];

          if (forecastedFor < forecastedAt) continue; // exclude points of past data.
          /*
          On 2024-12-25 was this: it should not grow any more because of the previous line.
          
          SELECT     COUNT(*) AS Expr1, SrcId, SiteId FROM        PointFore WHERE     (ForecastedFor < ForecastedAt) GROUP BY SrcId, SiteId

                70	omt	ome
              2264	omt	phc
              2029	owa	phc
          */

          if (await _dbx.PointFore.AnyAsync(d =>
              d.SrcId == srcId &&
              d.SiteId == siteId &&
              d.MeasureValue == siteData.Hourly.Temperature2m[h] && // _store only changes in recalculation results
              d.MeasureId == measureId &&
              d.ForecastedFor == forecastedFor) == false)
          {
            _ = _dbx.PointFore.Add(new PointFore
            {
              SrcId = srcId,
              SiteId = siteId,
              MeasureId = measureId,
              MeasureValue = siteData.Hourly.Temperature2m[h],
              ForecastedFor = forecastedFor,
              ForecastedAt = forecastedAt,
              Note64 = "[early runs]",
              CreatedAt = now
            });
          }
        };

        WriteLine($"■■ {await _dbx.SaveChangesAsync()} rows saved ■■");
        return;
      }      
      catch (InvalidOperationException ex) { ex.Pop(_lgr); }
      catch (Exception ex) { ex.Pop(_lgr); }
    }
  }
  internal static async Task AddPast24hrToDB_EnvtCa(ILogger _lgr, WeatherxContext _dbx, string siteId, List<MeteoDataMy>? sitePast, string srcId = "eca", string measureId = "tar")
  {
    ArgumentNullException.ThrowIfNull(sitePast, $"@@@@@@@@@ {nameof(sitePast)}");

    for (var i = 0; i < 10; i++)
    {
      try
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

        WriteLine($"■■ {await _dbx.SaveChangesAsync()} rows saved ■■");
        return;
      }
      catch (InvalidOperationException ex) { WriteLine($"WARN: P{i,3} {ex.Message}"); await Task.Delay(1000); bpr.Warn(); }
      catch (Exception ex) { ex.Pop(_lgr); }
    }
  }

  internal static DateTimeOffset EnvtCaDate(string yyyyMMddHHmm)
  {
    if (!DateTime.TryParseExact(yyyyMMddHHmm, new string[] { "yyyyMMddHHmm", "yyyyMMddHHmmss" }, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var utc)) //var lcl = DateTime.ParseExact(yyyyMMddHHmm, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToLocalTime(); //tu: UTC to Local time.
      throw new ArgumentOutOfRangeException(nameof(yyyyMMddHHmm), yyyyMMddHHmm, "■─■─■ Can you imagine?!?!?!");

    return utc.ToLocalTime(); //tu: UTC to Local time.
  }

  internal static DateTimeOffset EnvtCaDate(dateStampType dateStampType)
  {
    return !DateTimeOffset.TryParseExact($"{dateStampType.year}-{dateStampType.month.Value}-{dateStampType.day.Value} {dateStampType.hour.Value}:{dateStampType.minute}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt)
      ? throw new ArgumentException(nameof(dateStampNameType))
      : dt;
  }

  internal static void RefillForeEnvtCa(ObservableCollection<DataPoint> points, siteData? siteDt)
  {
    ArgumentNullException.ThrowIfNull(siteDt, $"@@@@@@@@@ {nameof(siteDt)}");

    siteDt.hourlyForecastGroup.hourlyForecast?.ToList().ForEach(x => points.Add(new DataPoint(DateTimeAxis.ToDouble(PlotViewModelHelpers.EnvtCaDate(x.dateTimeUTC).DateTime), double.Parse(x.temperature.Value))));
  }

  internal static async Task<(List<MeteoDataMy> bvl, List<MeteoDataMy> pea)> GetPast24hrFromEC()
  {
    Past24hrHAP p24 = new();
    var pea = await p24.GetIt(Cnst._Past24YYZ);
    var bvl = await p24.GetIt(Cnst._Past24OKN);
    return (bvl, pea);
  }
  internal static async Task<(siteData? sitedataMiss, siteData? sitedataVghn)> GetFore24hrFromEC()
  {
    var sitedataMiss = await OpenWea.GetEnvtCa(Cnst._mississ);
    var sitedataVghn = await OpenWea.GetEnvtCa(Cnst._kingcty);

    return (sitedataMiss, sitedataVghn);
  }

  internal static async Task<List<MeteoDataMy>> GetPast24hrFromEC(string url) => await new Past24hrHAP().GetIt(url);
  internal static async Task<siteData?> GetFore24hrFromEC(string site) => await OpenWea.GetEnvtCa(site);
}