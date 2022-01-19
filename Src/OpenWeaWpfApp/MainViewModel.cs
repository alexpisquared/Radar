using System.Collections.ObjectModel;
using OpenWeather2022;
using OpenWeather2022.Response;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using static OpenWeather2022.OpenWea;

namespace OpenWeaWpfApp;

public class MainViewModel
{
  readonly IConfigurationRoot _config;
  readonly OpenWea _opnwea;
  readonly int _m = -06 * 3600, _d = +00 * 3600, _e = +06 * 3600, _n = +11 * 3600, _d3r = 4, _d3c = 600, _h = 999, _windClr = 333, _popClr = 0;

  public MainViewModel()
  {
    _config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    _opnwea = new OpenWea();
  }

  public async Task<bool> PopulateAsync()
  {
    //await Task.Delay(999); no diff
    await PopulateScatModelAsync();
    await PopulateFuncModelAsync();
    //await PopulateOpenWeathAsync(); -- extra calls 
    return true;
  }

  async Task PopulateScatModelAsync()
  {
    PointsGust.Clear();
    PointsWind.Clear();
    PointsTemp.Clear();
    PointsFeel.Clear();
    PointsFeel.Clear();
    PointsSunT.Clear();

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
      TextColor = OxyColors.WhiteSmoke
    });
    ScatModel.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Rainbow(1000), TextColor = OxyColors.WhiteSmoke });

    oca.hourly.ToList().ForEach(x =>
    {
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._1h ?? 0, x.snow?._1h ?? 0, _d3c)); // either null or 0 so far.
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10, x.pop * 10, _popClr));

      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp, _d3r, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.feels_like, 2, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed, _d3r, _windClr));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust, 3, _windClr));

      //if (x.snow?._1h == 0) WriteLine(x.snow); else WriteLine(x.snow);

      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.temp));
      PointsFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.feels_like));
      PointsWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed));
      PointsGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust));
      PointsPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
    });

    d53.list.Where(d => d.dt > oca.hourly.Max(d => d.dt)).ToList().ForEach(x =>
    {
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.snow?._3h ?? 0, x.snow?._3h ?? 0, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10, x.pop * 10, _popClr));

      lsD5H3y.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));

      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp, _d3r, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like, 2, _d3c));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed, _d3r, _windClr));
      scaters.Points.Add(new(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust, 2, _windClr));

      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.temp));
      PointsFeel.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.main.feels_like));
      PointsWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.speed));
      PointsGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind.gust));
      PointsPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
    });

    PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueMax - 5));
    PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueMin));
    oca.daily.ToList().ForEach(x =>
    {
      if (OpenWea.UnixToDt(x.sunset) > DateTime.Now)
      {
        PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunrise)), valueMin));
        PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunrise)), valueMax));
        PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunset)), valueMax));
        PointsSunT.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.sunset)), valueMin));
      }
    });

    oca.daily
      .Where(d => d.dt > d53.list.Max(d => d.dt))
      .ToList().ForEach(x =>
    {
      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), x.temp.morn));
      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), x.temp.day));
      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), x.temp.eve));
      PointsTemp.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), x.temp.night));
      PointsWind.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed));
      PointsGust.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust));
      PointsPopr.Add(new DataPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.pop * 10));
    });

    oca.daily.ToList().ForEach(x =>
    {
      SctrPtTemp.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _m)), value: 10, tag: $"{x.temp.morn} ", y: x.temp.morn));
      SctrPtTemp.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _d)), value: 10, tag: $"{x.temp.day}  ", y: x.temp.day));
      SctrPtTemp.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _e)), value: 10, tag: $"{x.temp.eve}  ", y: x.temp.eve));
      SctrPtTemp.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt + _n)), value: 10, tag: $"{x.temp.night}", y: x.temp.night)); 
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


        scaters.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_speed, 3, _windClr));
        scaters.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWea.UnixToDt(x.dt)), x.wind_gust, 2, _windClr));
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

  public ObservableCollection<ScatterPoint> SctrPtTemp { get; } = new ObservableCollection<ScatterPoint>();
  public ObservableCollection<DataPoint> PointsTemp { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFeel { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsWind { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsGust { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsSunT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsPopr { get; } = new ObservableCollection<DataPoint>();

  public ObservableCollection<DataPoint> PointsTempC { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFeelC { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsTempT { get; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFeelT { get; } = new ObservableCollection<DataPoint>();

  public PlotModel FuncModel { get; private set; } = new PlotModel { Title = "Function Srs", Background = OxyColor.FromUInt32(123456), LegendTitleColor = OxyColor.FromUInt32(123456) };
  public PlotModel ScatModel { get; private set; } = new PlotModel { Title = "Scatter Srs" };

  public string Title { get; } = "Main VM !@##!@#!@";

}
///todo: https://oxyplot.readthedocs.io/en/latest/models/series/ScatterSeries.html
///https://docs.microsoft.com/en-us/answers/questions/22863/how-to-customize-charts-in-wpf-using-systemwindows.html
///https://docs.microsoft.com/en-us/answers/questions/10086/draw-chart-with-systemwindowscontrolsdatavisualiza.html
