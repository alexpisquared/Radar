using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using OpenWeather2022;
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
    Points1 = new ObservableCollection<DataPoint>();
    Points2 = new ObservableCollection<DataPoint>();

    Populate0();
    PopulateB();
  }

  async void Populate0() { await Task.Delay(3); }

  async void PopulateB()
  {
    var config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    WriteLine($"---   WhereAmI: '{config["WhereAmI"]}'       {config["AppSecrets:MagicNumber"]}");

    var oo = new OpenWeatherRevisit2022();
    var oca = await oo.GetIt(config["AppSecrets:MagicNumber"]);

    ArgumentNullException.ThrowIfNull(oca);

    oca?.hourly.ToList().ForEach(x =>
    {
      Points1.Add(new(x.dt, x.temp));
      //Points2.Add(new(x.dt, x.feels_like));
    });

    oca?.daily.Where(d => d.dt > oca.hourly.Max(d => d.dt)).ToList().ForEach(x =>
      {
        Points1.Add(new(x.dt + 06 * 3600, x.temp.morn));
        Points1.Add(new(x.dt + 12 * 3600, x.temp.day));
        Points1.Add(new(x.dt + 18 * 3600, x.temp.eve));
        Points1.Add(new(x.dt + 23 * 3600, x.temp.night));
      //Points2.Add(new(x.dt, x.feels_like.morn));
      //Points2.Add(new(x.dt, x.feels_like.night));
    });
  }

  public string Title { get; private set; }

  public ObservableCollection<DataPoint> Points1 { get; private set; }
  public ObservableCollection<DataPoint> Points2 { get; private set; }

  public PlotModel MyModel { get; private set; }
}
///todo: https://oxyplot.readthedocs.io/en/latest/models/series/ScatterSeries.html
///https://docs.microsoft.com/en-us/answers/questions/22863/how-to-customize-charts-in-wpf-using-systemwindows.html
///https://docs.microsoft.com/en-us/answers/questions/10086/draw-chart-with-systemwindowscontrolsdatavisualiza.html
