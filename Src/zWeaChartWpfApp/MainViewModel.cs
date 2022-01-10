using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using OpenWeather2022;
using OpenWeather2022.Response;
using OxyPlot;
using OxyPlot.Series;

namespace zWeaChartWpfApp;

public class MainViewModel
{
  public MainViewModel()
  {
    MyModel = new PlotModel { Title = "Example 1" };
    MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));

    Title = "Example 2";


    PopulateAsync();
  }

  async void PopulateAsync()
  {
    var config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    WriteLine($"---   WhereAmI: '{config["WhereAmI"]}'       {config["AppSecrets:MagicNumber"]}");

    var oo = new OpenWeatherRevisit2022();
    var occ = await oo.GetIt(config["AppSecrets:MagicNumber"], 43.8374229, -79.4961442); // PHC107
    var oct = await oo.GetIt(config["AppSecrets:MagicNumber"], 43.7181557, -79.5181414); // 400x401

    DrawSeries(occ, PointsTC, PointsFC);
    DrawSeries(oct, PointsTT, PointsFT);
  }

  void DrawSeries(RootobjectOneCallApi? ocv, ObservableCollection<DataPoint> pointsTemp, ObservableCollection<DataPoint> pointsFeel)
  {
    ArgumentNullException.ThrowIfNull(ocv);

    ocv?.hourly.ToList().ForEach(x =>
    {
      pointsTemp.Add(new(x.dt, x.temp));
      pointsFeel.Add(new(x.dt, x.feels_like));
    });

    ocv?.daily.Where(d => d.dt > ocv.hourly.Max(d => d.dt)).ToList().ForEach(x =>
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

  public string Title { get; private set; }
  public ObservableCollection<DataPoint> PointsTV { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFV { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsTC { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFC { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsTT { get; private set; } = new ObservableCollection<DataPoint>();
  public ObservableCollection<DataPoint> PointsFT { get; private set; } = new ObservableCollection<DataPoint>();

  public PlotModel MyModel { get; private set; }
}
///todo: https://oxyplot.readthedocs.io/en/latest/models/series/ScatterSeries.html
///https://docs.microsoft.com/en-us/answers/questions/22863/how-to-customize-charts-in-wpf-using-systemwindows.html
///https://docs.microsoft.com/en-us/answers/questions/10086/draw-chart-with-systemwindowscontrolsdatavisualiza.html
