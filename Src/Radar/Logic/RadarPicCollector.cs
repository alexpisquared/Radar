using StandardLib.Extensions;
using AsLink;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WebScrap;

namespace RadarPicCollect
{
  public class RadarPicCollector
  {
    static readonly int GmtOffset = (int)((DateTime.UtcNow - DateTime.Now).TotalHours + .1);  //4 in summer, 5 in winter
    public static string UrlForModTime(string rsRainOrSnow, DateTime d, string station, bool isFallbackCAPPI, bool isFallbackCOMP) => EnvCanRadarUrlHelper.GetRadarUrl(d); // , rsRainOrSnow, station, isFallbackCAPPI, isFallbackCOMP);

    static int _stationIndex = 0;
    const int _backLenLive = 48; // 8hrs    usually there was 24 available; (4hr-10min) coverage for radar; more for sattelite.
    int _backLenCur = 0; // 300 does not work anymore: there seems to be no access to the historical data - only to immediate last 24 pics/4 hours;//300==48 hours; 240;//40hrs 

    List<PicDetail> _picDtlList = new();
    readonly SortedList<string, Bitmap> _urlPicList = new(StringComparer.CurrentCultureIgnoreCase);

    static readonly string[] _station = { "WKR",								     // king city - RAIN/SNOW
																          "WSO" };                   // london    - RAIN/SNOW											
    readonly System.Drawing.Point[] _stationOffset = { new System.Drawing.Point(0,0),				 // king city - RAIN/SNOW
															          new System.Drawing.Point(-292,131) };  // london    - RAIN/SNOW

    public int StationCount => _station.Length;

    public string DownloadRadarPics(int max = 99999)
    {
      _backLenCur = _backLenLive;
      int period = 6;

      var gmt10 = RoundBy10min(DateTime.UtcNow, period); // DateTime.Now.AddHours(4));//.AddMinutes(-10 * backLen);//shows 1 hr behind at winter time (Dec2007)  Debug.Assert(DateTime.Now.AddHours(4) == DateTime.UtcNow); //Apr2015

      for (var back = _backLenCur; back >= 0; back--)
      {
        getPic(back, gmt10, period);
        if (_urlPicList.Count >= max) break;
      }

      return string.Format("   <{0} out of {1} Pics Loaded @{2}>", _picDtlList.Count, _backLenCur, DateTime.Now.ToString("d HH:mm"));
    }
    public string DownloadRadarPics_MostRecent_RAIN_only(int max, int period)
    {
      var gmt10 = RoundBy10min(DateTime.UtcNow); // DateTime.Now.AddHours(4));//.AddMinutes(-10 * backLen);//shows 1 hr behind at winter time (Dec2007)  Debug.Assert(DateTime.Now.AddHours(4) == DateTime.UtcNow); //Apr2015

      for (var times10minBack = 0; times10minBack < _backLenLive && _urlPicList.Count < max; times10minBack++)
        getPic(times10minBack, gmt10, period, "RAIN");

      return string.Format("   <{0} out of {1} Pics Loaded @{2}>", _picDtlList.Count, _backLenCur, DateTime.Now.ToString("d HH:mm"));
    }

    void getPic(int times10minBack, DateTime gmt10Now, int period, string? rainOrSnow = null)
    {
      if (!string.IsNullOrEmpty(rainOrSnow))
        RainOrSnow = rainOrSnow;

#if ___DEBUG //annoying during video watching
      Bpr.BeepFD(11000 + 50 * times10minBack, 40);
#endif
      Bitmap? pic = null;
      var url = "??";

      try
      {
        _stationIndex = 0;
        var dt = gmt10Now.AddMinutes(-period * times10minBack);


        if (times10minBack < _backLenLive)
        {
          if ((pic = WebScraperBitmap.DownloadImageCached(url = UrlForModTime(RainOrSnow, dt, _station[_stationIndex], false, true).Split('|')[0])) == null /*&&
              (pic = WebScraperBitmap.DownloadImageCached(url = UrlForModTime(RainOrSnow, dt, _station[_stationIndex], false, false).Split('|')[0])) == null &&
              (pic = WebScraperBitmap.DownloadImageCached(url = UrlForModTime(RainOrSnow, dt, _station[_stationIndex], true, true).Split('|')[0])) == null &&
              (pic = WebScraperBitmap.DownloadImageCached(url = UrlForModTime(RainOrSnow, dt, _station[_stationIndex], true, false).Split('|')[0])) == null*/)
            return;

          if (_urlPicList.ContainsKey(url)) { _urlPicList.Remove(url); Debug.WriteLine(string.Format("+> {0} already there.", url)); }
          _urlPicList.Add(url, pic);
          _picDtlList.Add(new PicDetail(pic, dt.AddHours(-GmtOffset), _station[_stationIndex], _stationOffset[_stationIndex], WebScraper.GetCachedFileNameFromUrl_(url.Split('|')[0], false)));
        }
        else
        {
          if ((pic = WebScraperBitmap.LoadImageFromFile(url = UrlForModTime(RainOrSnow, dt, _station[_stationIndex], true, true).Split('|')[0])) == null /*&&
              (pic = WebScraperBitmap.LoadImageFromFile(url = UrlForModTime(RainOrSnow, dt, _station[_stationIndex], true, false).Split('|')[0])) == null &&
              (pic = WebScraperBitmap.LoadImageFromFile(url = UrlForModTime(RainOrSnow, dt, _station[_stationIndex], false, true).Split('|')[0])) == null &&
              (pic = WebScraperBitmap.LoadImageFromFile(url = UrlForModTime(RainOrSnow, dt, _station[_stationIndex], false, false).Split('|')[0])) == null*/)
            return;

          if (_urlPicList.ContainsKey(url)) { _urlPicList.Remove(url); Debug.WriteLine(string.Format("+> {0} already there.", url)); }
          _urlPicList.Add(url, pic);
          _picDtlList.Add(new PicDetail(pic, dt.AddHours(-GmtOffset), _station[_stationIndex], _stationOffset[_stationIndex], WebScraper.GetCachedFileNameFromUrl_(url.Split('|')[0], false)));

          //pic = WebScraperBitmap.DownloadImageCached(urlHist.Split('|')[0]);//GetWebImageFromCache(urlHist.Split('|')[0]);
        }
      }
      catch (Exception ex) { ex.Log(); }
      finally
      {
        report(times10minBack, url, pic, " cache");
      }
    }

    static void report(int back, string url_time_, Bitmap? pic, string src) => Debug.WriteLine($"{back,3}/{_backLenLive} {url_time_}: from   {src}: {(pic == null ? " - Unable to get this pic" : " + SUCCESS")}");

    public static string RainOrSnow
    {
      get
      {
        if (_rainOrSnow == null)
        {
          var url = string.Format("https://weather.gc.ca/radar/index_e.html?id={0}", _station[_stationIndex]);
          var htm = WebScraper.GetHtmlFromCacheOrWeb(url, TimeSpan.FromHours(1));

          _rainOrSnow = htm.IndexOf("SNOW") > 0 ? "SNOW" : "RAIN";
        }
        return _rainOrSnow;
      }
      set
      {
        if (_rainOrSnow != value)
        {
          _rainOrSnow = value;
        }
      }
    }

    static string? _rainOrSnow = null;//(DateTime.Now.DayOfYear < 72 || DateTime.Now.Month == 12) ? "SNOW" : "RAIN";

    public string DownloadRadarPicsNextBatch(int stationIndex = 0) { _stationIndex = stationIndex; return DownloadRadarPics(); }

    public List<PicDetail> Pics
    {
      get => _picDtlList;
      set => _picDtlList = value;
    }
    public int IdxTime(DateTime time)
    {
      for (var i = 0; i < _picDtlList.Count; i++)
      {
        if (_picDtlList[i].ImageTime == time)
          return i;
      }
      return -1;
    }
    public PicDetail? Time(DateTime time)
    {
      foreach (var pd in _picDtlList)
      {
        if (pd.ImageTime == time)
          return pd;
      }
      return null;
    }
    public static DateTime RoundBy10min(DateTime dt, int period = 10)
    {
      dt = dt.AddTicks(-dt.Ticks % (period * TimeSpan.TicksPerMinute));
      return dt;
    }
    public static TimeSpan RoundBy10min(TimeSpan dt)
    {
      dt = dt.Add(TimeSpan.FromTicks(-dt.Ticks % (10 * TimeSpan.TicksPerMinute)));
      return dt;
    }

    public static Bitmap? GetBmpLclTime(DateTime lcltime)
    {
      var gmt = RoundBy10min(lcltime.AddHours(GmtOffset));
      return GetBmp(gmt);
    }
    public static Bitmap? GetLatestBmp()
    {
      var gmtNow = RoundBy10min(DateTime.UtcNow);
      Debug.Assert((int)((DateTime.UtcNow - DateTime.Now.AddHours(RadarPicCollector.GmtOffset)).TotalHours) == 0);

      try
      {
        for (var i = 0; i < 18; i++) // in the last 3 hours.
        {
          var dt = gmtNow.AddMinutes(-10 * i);

          var bmp = RadarPicCollector.GetBmp(dt);
          if (bmp != null)
            return bmp;
        }
      }
      catch (Exception ex) { ex.Log(); }

      return null;
    }
    public static Bitmap? GetBmp(DateTime gmtTime, string station = "WSO")
    {
      if (_cache.ContainsKey(gmtTime)) return _cache[gmtTime];

      //string rainOrSnow = (DateTime.Now.DayOfYear < 70 || DateTime.Now.Month == 12) ? "SNOW" : "RAIN";//todo: maybe temperature based.
      Bitmap? bmp = null;

      try
      {
        if (bmp == null) bmp = getFromCacheOrWeb(UrlForModTime(RainOrSnow, gmtTime, "WKR", false, false).Split('|')[0]);
        if (bmp == null) bmp = getFromCacheOrWeb(UrlForModTime(RainOrSnow, gmtTime, "WKR", true, true).Split('|')[0]);
        if (bmp == null) bmp = getFromCacheOrWeb(UrlForModTime(RainOrSnow, gmtTime, "WSO", false, false).Split('|')[0]);
        if (bmp == null) bmp = getFromCacheOrWeb(UrlForModTime(RainOrSnow, gmtTime, "WSO", true, true).Split('|')[0]);
      }
      catch (Exception ex) { ex.Log(); }
      finally
      {
        if (bmp != null && !_cache.ContainsKey(gmtTime)) _cache.Add(gmtTime, bmp);
      }

      return bmp;
    }

    static Bitmap? getFromCacheOrWeb(string url)
    {
      var bmp = WebScraperBitmap.LoadImageFromFile(url);
      if (bmp != null) return bmp;

      bmp = WebScraperBitmap.DownloadImageCached(url);
      return bmp;
    }

    static readonly SortedList<DateTime, Bitmap> _cache = new();

    public static string GetRainForecastReport()
    {
      var report = "";
      var gmtNow = RadarPicCollector.RoundBy10min(DateTime.UtcNow);

      //Debug.Assert((int)((DateTime.UtcNow - DateTime.Now.AddHours(RadarPicCollector._GmtOffset)).TotalHours) == 0);

      try
      {
        const int max = 2;
        var tm = new DateTime[max];
        var mh = new double[max];
        for (int j = max - 1, i = 0; i < 72 && j >= 0; i++)
        {
          var dt = gmtNow.AddMinutes(-10 * i);

          var bmp = RadarPicCollector.GetBmp(dt);
          if (bmp != null)
          {
            tm[j] = dt.AddHours(-RadarPicCollector.GmtOffset);
            mh[j] = PicMea.CalcMphInTheArea(bmp, dt);
            j--;
          }
        }

        var dMh = mh[0] - mh[max - 1];
        var dTm = tm[0] - tm[max - 1];
        var kh = dMh / dTm.TotalHours;
        var tmNow = DateTime.Now;
        var mhNow = mh[0] + kh * (tmNow - tm[0]).TotalHours;
        var tm30m = DateTime.Now.AddMinutes(30);
        var mh30m = mh[0] + kh * (tm30m - tm[0]).TotalHours;

        var f = "  {0:HH:mm}:  {1:N3} \n";
        report += string.Format("{0:MMM d:}\n", tmNow);
        report += string.Format(f, tm[0], mh[0]);
        report += string.Format(f, tm[1], mh[1]);
        report += string.Format(f, tmNow, mhNow);
        report += string.Format(f, tm30m, mh30m);
        if (dMh <= 0)
          report += string.Format("More is coming.");
        else
          report += string.Format("End at {0:HH:mm} or in {1:N1} hours.", tmNow.AddHours(-mhNow / kh), (-mhNow / kh));
      }
      catch (Exception ex) { report = ex.Message; }

      return report;
    }
  }
}