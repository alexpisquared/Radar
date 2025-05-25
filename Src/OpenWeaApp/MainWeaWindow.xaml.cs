using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using StandardContractsLib;

namespace OpenWeaApp;
public partial class MainWeaWindow
{
  readonly ILogger _lgr;
  readonly IBpr _bpr;

  public MainWeaWindow(ILogger lgr, IBpr bpr)
  {
    InitializeComponent();

    _lgr = lgr;
    _bpr = bpr;

    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.C: _bpr.Error(); break; // ((PlotViewModel)DataContext).ClearPlot(); break;
        case Key.I: _bpr.Error(); break; // plotBR.InvalidatePlot(true); break;
        case Key.J: _bpr.Error(); break; // plotBR.InvalidatePlot(false); break;
        case Key.R: _bpr.Error(); break; // ((PlotViewModel)DataContext).PopulateAll(null); goto case Key.I;
        default: await Task.Delay(333); break;
      }
    };

    //Topmost = Debugger.IsAttached;

    KeepOpenReason = null; // nothing to hold on to.

    Title = $"OpenWeaWpfApp - {string.Join(',', Environment.GetCommandLineArgs())}";
  }

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    try
    {
      //((PlotViewModel)DataContext).PopulateAll("Silent");  // only lines chart is drawn.
      await Task.Delay(1);
    }
    catch (Exception)
    {
      //ex.Pop(_lgr);
    }
  }
  private void OnClose(object sender, RoutedEventArgs e) => Close();

  void OnActivated(object sender, EventArgs e) { } // adar1.IsPlaying = true;  /*_syn.SpeakFAF("Activated", volumePercent: 5);*/
  void OnDeActivtd(object sender, EventArgs e) { } // radar1.IsPlaying = false; /*_syn.SpeakFAF("Deactivated", volumePercent: 5);*/
}
