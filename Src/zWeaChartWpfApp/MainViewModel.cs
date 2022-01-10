using System.Collections.ObjectModel;
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
  private OpenWeatherRevisit2022 _opnwea;

  public MainViewModel()
  {
    _config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    _opnwea = new OpenWeatherRevisit2022();
  }

  public async Task<bool> PopulateAsync()
  {
    await Task.Yield();
    await
    PopulateScatModelAsync();
    await PopulateFuncModel();
    await PopulateOpenWeath();
    return true;
  }

  async Task PopulateFuncModel()
  {
    await Task.Yield();
    FuncModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
  }

  async Task PopulateScatModelAsync()
  {
    var ocv = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], 43.8374229, -79.4961442); // PHC107
    ArgumentNullException.ThrowIfNull(ocv);

    var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

    var minValue = DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(ocv.hourly.Min(d => d.dt - 3600)));
    var maxValue = DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(ocv.daily.Max(d => d.dt + 3600)));

    ScatModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, StringFormat = "ddd HH" });

    ocv.hourly.ToList().ForEach(x =>
    {
      scatterSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt)), x.temp, 6, 630));
      scatterSeries.Points.Add(new ScatterPoint(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt)), x.feels_like, 3, 530));
    });

    ocv.daily./*Where(d => d.dt > ocv.hourly.Max(d => d.dt)).*/ToList().ForEach(x =>
    {
      scatterSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt - 08 * 3600)), x.temp.morn, 4, 0));
      scatterSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt + 00 * 3600)), x.temp.day, 4, 750));
      scatterSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt + 06 * 3600)), x.temp.eve, 4, 1000));
      scatterSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt + 11 * 3600)), x.temp.night, 4, 333));
      scatterSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt - 08 * 3600)), x.feels_like.morn, 2, 0));
      scatterSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt + 00 * 3600)), x.feels_like.day, 2, 750));
      scatterSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt + 06 * 3600)), x.feels_like.eve, 2, 1000));
      scatterSeries.Points.Add(new(DateTimeAxis.ToDouble(OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt + 11 * 3600)), x.feels_like.night, 2, 333));
    });

    ScatModel.Series.Add(scatterSeries);
    ScatModel.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Rainbow(1000) });
  }

  async Task PopulateOpenWeath()
  {
    var occ = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], 43.8374229, -79.4961442); // PHC107
    var oct = await _opnwea.GetIt(_config["AppSecrets:MagicNumber"], 43.7181557, -79.5181414); // 400x401

    DrawSeries(occ, PointsTC, PointsFC);
    DrawSeries(oct, PointsTT, PointsFT);
  }

  void DrawSeries(RootobjectOneCallApi? ocv, ObservableCollection<DataPoint> pointsTemp, ObservableCollection<DataPoint> pointsFeel)
  {
    ArgumentNullException.ThrowIfNull(ocv);

    ocv.hourly.ToList().ForEach(x =>
    {
      pointsTemp.Add(new(x.dt, x.temp));
      pointsFeel.Add(new(x.dt, x.feels_like));
    });

    ocv.daily.Where(d => d.dt > ocv.hourly.Max(d => d.dt)).ToList().ForEach(x =>
    {
      pointsTemp.Add(new(x.dt + 06 * 3600, x.temp.morn));
      pointsTemp.Add(new(x.dt + 12 * 3600, x.temp.day));
      pointsTemp.Add(new(x.dt + 18 * 3600, x.temp.eve));
      pointsTemp.Add(new(x.dt + 23 * 3600, x.temp.night));
      pointsFeel.Add(new(x.dt + 06 * 3600, x.feels_like.morn));
      pointsFeel.Add(new(x.dt + 12 * 3600, x.feels_like.day));
      pointsFeel.Add(new(x.dt + 18 * 3600, x.feels_like.eve));
      pointsFeel.Add(new(x.dt + 23 * 3600, x.feels_like.night));
    });
  }

  public string Title { get; private set; } = "Example 2";
  public ObservableCollection<DataPoint> PointsTV { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFV { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsTC { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFC { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsTT { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFT { get; private set; } = new ObservableCollection<DataPoint>();

  public PlotModel FuncModel { get; private set; } = new PlotModel { Title = "FunctionSeries Example" };
  public PlotModel ScatModel { get; private set; } = new PlotModel { Title = "ScatterSeries" };
}
///todo: https://oxyplot.readthedocs.io/en/latest/models/series/ScatterSeries.html
///https://docs.microsoft.com/en-us/answers/questions/22863/how-to-customize-charts-in-wpf-using-systemwindows.html
///https://docs.microsoft.com/en-us/answers/questions/10086/draw-chart-with-systemwindowscontrolsdatavisualiza.html
