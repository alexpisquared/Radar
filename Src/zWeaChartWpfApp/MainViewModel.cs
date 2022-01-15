﻿using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using OpenWeather2022;
using OpenWeather2022.Response;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace zWeaChartWpfApp;

public class MainViewModel
{
  IConfigurationRoot _config;
  OpenWea _opnwea;
  int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600;

  public MainViewModel()
  {
    _config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    _opnwea = new OpenWea();
  }

  public async Task<bool> PopulateAsync()
  {
    await PopulateScatModelAsync();
    await PopulateFuncModelAsync();
    await PopulateOpenWeathAsync();
    return true;
  }

  async Task PopulateScatModelAsync()
  {
    var owd = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], 43.8374229, -79.4961442); // PHC107
    ArgumentNullException.ThrowIfNull(owd);

    var sctrSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
    var lineSeries = new LineSeries { MarkerType = MarkerType.Triangle };

    var timeMin = DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(owd.daily.Min(d => d.dt - 07 * 3600)));
    var timeMax = DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(owd.daily.Max(d => d.dt + 12 * 3600)));
    var valueMin = owd.daily.Min(r => r.temp.min);
    var valueMax = owd.daily.Max(r => r.temp.max);

    ScatModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = timeMin, Maximum = timeMax, StringFormat = "ddd HH" });

    owd.hourly.ToList().ForEach(x =>
    {
      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.temp, 6, 630));
      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.feels_like, 3, 530));

      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.wind_speed, 3, 300));
      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.wind_gust, 2, 300));

      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.temp));
      PointsFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.feels_like));
      PointsWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.wind_speed));
      PointsGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.wind_gust));
    });

    PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueMax - 5));
    PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueMin));

    owd.daily.Where(d => d.dt > owd.hourly.Max(d => d.dt)).ToList().ForEach(x =>
    {
      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _m)), x.temp.morn));
      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _d)), x.temp.day));
      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _e)), x.temp.eve));
      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _n)), x.temp.night));
      PointsWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.wind_speed));
      PointsGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.wind_gust));
    });

    owd.daily.ToList().ForEach(x =>
    {
      if (OpenWea.UnixTimeStampToDateTime(x.sunset) > DateTime.Now)
      {
        PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.sunrise)), valueMin));
        PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.sunrise)), valueMax));
        PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.sunset)), valueMax));
        PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.sunset)), valueMin));
      }

      sctrSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _m)), x.temp.morn, 4, 0));
      sctrSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _d)), x.temp.day, 4, 750));
      sctrSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _e)), x.temp.eve, 4, 1000));
      sctrSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _n)), x.temp.night, 4, 333));

      sctrSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _m)), x.feels_like.morn, 2, 0));
      sctrSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _d)), x.feels_like.day, 2, 750));
      sctrSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _e)), x.feels_like.eve, 2, 1000));
      sctrSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _n)), x.feels_like.night, 2, 333));

      lineSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _m)), x.temp.morn));
      lineSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _d)), x.temp.day));
      lineSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _e)), x.temp.eve));
      lineSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt + _n)), x.temp.night));

      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.wind_speed, 3, 300));
      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.wind_gust, 2, 300));
      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.pop * 10,x.pop, 700));
      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.snow, x.snow, 990));
      sctrSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixTimeStampToDateTime(x.dt)), x.rain, x.rain, 500));
    });

    ScatModel.Series.Add(sctrSeries);
    ScatModel.Series.Add(lineSeries);

    ScatModel.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Rainbow(1000) });
  }
  async Task PopulateOpenWeathAsync()
  {
    var occ = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], 43.8374229, -79.4961442); // PHC107
    var oct = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], 43.7181557, -79.5181414); // 400x401

    CopyOpenWeaToPointsLists(occ, PointsTempC, PointsFeelC);
    CopyOpenWeaToPointsLists(oct, PointsTempT, PointsFeelT);
  }
  async Task PopulateFuncModelAsync() { await Task.Yield(); FuncModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)")); }

  void CopyOpenWeaToPointsLists(RootobjectOneCallApi? ocv, ObservableCollection<DataPoint> pointsTemp, ObservableCollection<DataPoint> pointsFeel)
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

  public ObservableCollection<DataPoint> PointsTemp { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFeel { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsWind { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsGust { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsSunT { get; } = new ObservableCollection<DataPoint>();

  public ObservableCollection<DataPoint> PointsTempC { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFeelC { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsTempT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFeelT { get; } = new ObservableCollection<DataPoint>();

  public PlotModel FuncModel { get; private set; } = new PlotModel { Title = "FunctionSeries Example" };
  public PlotModel ScatModel { get; private set; } = new PlotModel { Title = "ScatterSeries" };

  public string Title { get; } = "Example 2";

}
///todo: https://oxyplot.readthedocs.io/en/latest/models/series/ScatterSeries.html
///https://docs.microsoft.com/en-us/answers/questions/22863/how-to-customize-charts-in-wpf-using-systemwindows.html
///https://docs.microsoft.com/en-us/answers/questions/10086/draw-chart-with-systemwindowscontrolsdatavisualiza.html
