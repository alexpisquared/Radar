using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    Points1 = new List<DataPoint>                              {
                                  new DataPoint(0, 4),
                                  new DataPoint(10, 13),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 16),
                                  new DataPoint(40, 12),
                                  new DataPoint(50, 12)
                              };
    Points2 = new List<DataPoint>                              {
                                  new DataPoint(0, 14),
                                  new DataPoint(10, 23),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 36),
                                  new DataPoint(40, 02),
                                  new DataPoint(50, 12)
                              };


    var config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    WriteLine($"---   WhereAmI: '{config["WhereAmI"]}'       {config["AppSecrets:MagicNumber"]}");

    var oo = new OpenWeatherRevisit2022();
    var oca = oo.GetIt(config["AppSecrets:MagicNumber"]).Result;

    oca?.hourly.ToList().ForEach(x => WriteLine($":> {OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt):ddd HH}  {x.temp,6:N1}  {x.feels_like,6:N1}  {x.wind_speed,5:N0}  {x}"));
    oca?.minutely.ToList().ForEach(x => WriteLine($":> {OpenWeatherRevisit2022.UnixTimeStampToDateTime(x.dt):ddd HH:mm}  {x.precipitation,5:N0}  {x}"));

  }

  public string Title { get; private set; }

  public IList<DataPoint> Points1 { get; private set; }
  public IList<DataPoint> Points2 { get; private set; }

  public PlotModel MyModel { get; private set; }
}
